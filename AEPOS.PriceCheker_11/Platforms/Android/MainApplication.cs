using AEPOS.PriceChecker_11;
using Android.App;
using Android.Runtime;

namespace AEPOS.PriceCheker_11
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
