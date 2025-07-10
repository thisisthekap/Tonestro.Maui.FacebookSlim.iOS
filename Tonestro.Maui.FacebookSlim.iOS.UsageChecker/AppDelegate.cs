namespace Tonestro.Maui.FacebookSlim.iOS.UsageChecker;

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    public override UIWindow? Window { get; set; }

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        CoreKitManagerSlim.Shared.EnableLoggingBehavior(LoggingBehaviorEnum.AccessTokens);
        CoreKitManagerSlim.Shared.EnableLoggingBehavior(LoggingBehaviorEnum.PerformanceCharacteristics);
        CoreKitManagerSlim.Shared.EnableLoggingBehavior(LoggingBehaviorEnum.AppEvents);
        CoreKitManagerSlim.Shared.EnableLoggingBehavior(LoggingBehaviorEnum.Informational);
        CoreKitManagerSlim.Shared.EnableLoggingBehavior(LoggingBehaviorEnum.CacheErrors);
        CoreKitManagerSlim.Shared.EnableLoggingBehavior(LoggingBehaviorEnum.UIControlErrors);
        CoreKitManagerSlim.Shared.EnableLoggingBehavior(LoggingBehaviorEnum.GraphAPIDebugWarning);
        CoreKitManagerSlim.Shared.EnableLoggingBehavior(LoggingBehaviorEnum.NetworkRequests);
        CoreKitManagerSlim.Shared.EnableLoggingBehavior(LoggingBehaviorEnum.DeveloperErrors);
        CoreKitManagerSlim.Shared.IsAdvertiserIdCollectionEnabled = true;
        CoreKitManagerSlim.Shared.FinishedLaunching(application, ConvertToGenericNSDictionary(launchOptions));

        // create a new window instance based on the screen size
        Window = new UIWindow(UIScreen.MainScreen.Bounds);

        // create a UIViewController with a single UILabel
        var vc = new UIViewController();
        vc.View!.AddSubview(new UILabel(Window!.Frame)
        {
            BackgroundColor = UIColor.SystemBackground,
            TextAlignment = UITextAlignment.Center,
            Text = "Hello, iOS!",
            AutoresizingMask = UIViewAutoresizing.All,
        });
        Window.RootViewController = vc;

        // make the window visible
        Window.MakeKeyAndVisible();

        return true;
    }

    public override void OnActivated(UIApplication application)
    {
        CoreKitManagerSlim.Shared.ActivateApp();
    }

    // ReSharper disable once InconsistentNaming
    private static NSDictionary<NSString, NSObject> ConvertToGenericNSDictionary(NSDictionary? options)
    {
        if (options == null)
        {
            return new NSDictionary<NSString, NSObject>();
        }

        var keys = options.Keys.Select(o => new NSString(o.ToString())).ToArray();
        var values = options.Values;
        return new NSDictionary<NSString, NSObject>(keys, values);
    }
}