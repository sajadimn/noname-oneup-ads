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
1. Add this package. It creates `AdMobAdsConfig`, `ApplovinAdsConfig` and `TapsellAdsConfig` in `Assets/Settings/Resources/Ads`.
2. The three ad SDKs, the Google and Mintegral adapters for AppLovin, the Tapsell Legacy adapter and the required scoped registries install automatically. The install continues by itself after each Unity reload. If it fails, for example with no internet, run `OneUp/Ads/Install SDKs`.
3. Enter your keys in the three config assets, and the Tapsell app key in Tapsell's Project Settings page. AdMob and AppLovin have separate Android and iOS keys.
4. Call `AdsManager.Instance.Init()`. No prefab or scene object is needed.

AdMob and AppLovin use the Google Consent package.
