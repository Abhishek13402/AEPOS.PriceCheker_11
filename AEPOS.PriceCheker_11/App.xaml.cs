using AEPOS.PriceChecker_11.Pages;
using AEPOS.PriceChecker_11.SQLiteClasses;

namespace AEPOS.PriceCheker_11
{
    public partial class App : Application
    {
        public static CacheDataRepo cacheDataRepo { get; private set; }
        public App(CacheDataRepo repo)
        {
            InitializeComponent();

            cacheDataRepo = repo;

            MainPage = new NavigationPage(new LoginPage());
            Current.UserAppTheme = AppTheme.Light;
        }

        // Optional: Handle application lifecycle events if needed
        protected override void OnStart()
        {
            base.OnStart();
            // Code to execute when the app starts
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            // Code to execute when the app goes to sleep
        }

        protected override void OnResume()
        {
            base.OnResume();
            // Code to execute when the app resumes from sleep
        }
    }
}
