using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;
using System.Reflection;
using ClassToHpp;

namespace project
{
    [Guid(ClassToHpp.PackageGuidString)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, flags: PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class ClassToHpp : AsyncPackage
    {
        public const string PackageGuidString = "d300a506-4a22-40e0-8966-9b146a525312";
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            HppPatch.ApplyPatch();
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }
    }
}
