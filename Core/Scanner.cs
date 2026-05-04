using System;
using System.Threading;
using ScumChecker.Core.Modules;

namespace ScumChecker.Core
{
    public sealed class Scanner
    {
        public event Action<ProgressInfo>? Progress;
        public event Action<string>? Log;
        public event Action<ScanItem>? ItemFound;

        private void EmitProgress(int percent, string stage) =>
            Progress?.Invoke(new ProgressInfo { Percent = Math.Clamp(percent, 0, 100), Stage = stage });

        private void EmitLog(string text) => Log?.Invoke(text);

        private void EmitItem(ScanItem item) => ItemFound?.Invoke(item);

        public ScanResult Run(CancellationToken ct)
        {
            var result = new ScanResult();

            IScanModule[] modules =
            [
                new ProcessesModule(),
                new NativeDriverMemoryModule(),
                new HwidModule(),
            ];

            EmitLog($"Modules: {modules.Length}");

            try
            {
                for (int i = 0; i < modules.Length; i++)
                {
                    ct.ThrowIfCancellationRequested();

                    var m = modules[i];
                    var basePct = (int)(i * 100.0 / modules.Length);
                    EmitProgress(basePct, m.Name);
                    EmitLog($"Running: {m.Name}");

                    try
                    {
                        foreach (var item in m.Run(ct))
                        {
                            ct.ThrowIfCancellationRequested();

                            result.Items.Add(item);
                            try
                            {
                                EmitItem(item);
                            }
                            catch (Exception ex)
                            {
                                EmitLog($"Item handler failed: {ex.GetType().Name}: {ex.Message}");
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // отмена во время конкретного модуля — пробрасываем наружу
                        throw;
                    }
                    catch (Exception ex)
                    {
                        // модуль упал — не валим весь скан, просто логируем и идём дальше
                        EmitLog($"Module failed: {m.Name} | {ex.GetType().Name}: {ex.Message}");
                    }
                }

                EmitProgress(100, "Done");
                EmitLog("Scan completed.");
                return result;
            }
            catch (OperationCanceledException)
            {
                EmitProgress(100, "Canceled");
                EmitLog("Scan canceled.");
                return result; // вернём то, что успели найти
            }
        }
    }
}
