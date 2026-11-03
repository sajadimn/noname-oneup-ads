Ads Unity Package
===

## Description
This package contains the ads manager for AdMob, AppLovin MAX and Tapsell. Keys stay in your project, never in this package.

## Dependencies
Add these to `Packages/manifest.json` before this package, or it cannot compile:
- [noname-oneup-singleton](https://github.com/sajadimn/noname-oneup-singleton)
- [noname-oneup-region](https://github.com/sajadimn/noname-oneup-region)
- [noname-oneup-google-consent](https://github.com/sajadimn/noname-oneup-google-consent)

## How to use
1. Add this package. It creates `AdMobAdsConfig`, `ApplovinAdsConfig` and `TapsellAdsConfig` in `Assets/Settings/Ads/Resources`.
2. The three ad SDKs, the Google and Mintegral adapters for AppLovin, the Tapsell Legacy adapter and the required scoped registries install automatically. The install continues by itself after each Unity reload. If it fails, for example with no internet, run `OneUp/Ads/Install SDKs`.
3. To change SDK versions, edit `Assets/Settings/Ads/AdsSdkVersionsConfig.asset`, then run `OneUp/Ads/Update SDKs`. Enter your keys in the three config assets, and the Tapsell app key in Tapsell's Project Settings page. AdMob and AppLovin have separate Android and iOS keys.
4. Call `AdsManager.Instance.Init(market)` with your store name, for example `"GooglePlay"`. `AdsRulesConfig` decides which network each store uses. No prefab or scene object is needed.

## Android setup (Tapsell)
Tapsell's Android library needs the app key at build time. Do this once in every game:
1. Project Settings > Player > Android > Publishing Settings: turn on **Custom Launcher Gradle Template**. This creates `Assets/Plugins/Android/launcherTemplate.gradle`.
2. Project Settings > Tapsell Mediation: enter this game's **App Key** from the Tapsell dashboard. Never copy another game's key.
3. Run **Assets > Tapsell Mediation > Force Refresh Keys**. It writes the key into the template between `// Tapsell Mediation Keys Start` and `// Tapsell Mediation Keys End`.
4. Commit `Assets/Plugins/Android/launcherTemplate.gradle` and `ProjectSettings/TapsellMediationSettings.json`.

Before every Android build, the package checks this and only reads files. The build stops with a clear message when the template is off, missing or has no Tapsell block, because Gradle would fail on `${TapsellMediationAppKey}`. An empty App Key, or a template key different from the one in Tapsell's settings, only logs a warning, because the build still works but Tapsell ads will not.

AdMob and AppLovin use the Google Consent package.
