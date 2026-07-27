namespace Tonestro.Maui.FacebookSlim.iOS.UsageChecker;

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    public override UIWindow? Window { get; set; }

    private readonly UILabel _infoLabel = new()
    {
        BackgroundColor = UIColor.SystemBackground,
        TextAlignment = UITextAlignment.Left,
        Font = UIFont.GetMonospacedSystemFont(14, UIFontWeight.Regular),
        Lines = 0,
        AutoresizingMask = UIViewAutoresizing.All,
    };

    private int _loggedEvents;

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
        var finishedLaunching =
            CoreKitManagerSlim.Shared.FinishedLaunching(application, ConvertToGenericNSDictionary(launchOptions));
        CoreKitManagerSlim.Shared.UserId = "usage-checker-42";

        // create a new window instance based on the screen size
        Window = new UIWindow(UIScreen.MainScreen.Bounds);

        var vc = new UIViewController();
        _infoLabel.Frame = Window!.Frame.Inset(24, 60);
        vc.View!.BackgroundColor = UIColor.SystemBackground;
        vc.View!.AddSubview(_infoLabel);
        Window.RootViewController = vc;

        RefreshInfo($"FinishedLaunching: {finishedLaunching}");

        // make the window visible
        Window.MakeKeyAndVisible();

        return true;
    }

    public override void OnActivated(UIApplication application)
    {
        CoreKitManagerSlim.Shared.ActivateApp();
        CoreKitManagerSlim.Shared.LogEvent("test",
            NSDictionary.FromObjectAndKey(new NSString("1234"), new NSString("asdf")));
        _loggedEvents++;
        CoreKitManagerSlim.Shared.LogEvent(AppEventNameEnum.ViewedContent, 4.2,
            NSDictionary.FromObjectAndKey(new NSString("1234"), new NSString("asdf")));
        _loggedEvents++;

        RefreshInfo("FinishedLaunching: True");
    }

    private void RefreshInfo(string launchState)
    {
        // every value below is read back through the binding from the native Facebook SDK
        _infoLabel.Text =
            $"""
             Facebook SDK via binding

             SdkVersion:
               {CoreKitManagerSlim.Shared.SdkVersion}
             {launchState}
             AnonymousId:
               {CoreKitManagerSlim.Shared.AnonymousId}
             UserId (round-trip):
               {CoreKitManagerSlim.Shared.UserId}
             AdvertiserIdCollection:
               {CoreKitManagerSlim.Shared.IsAdvertiserIdCollectionEnabled}
             LoginManagerSlim.Shared:
               {(LoginManagerSlim.Shared != null ? "resolved" : "null")}
             Events logged: {_loggedEvents}
             """;
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
