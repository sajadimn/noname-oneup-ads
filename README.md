Ads Unity Package
===

## Description
This package contains the ads manager for AdMob, AppLovin MAX and Tapsell. Keys stay in your project, never in this package.

## Dependencies
Add these to `Packages/manifest.json` before this package, or it cannot compile:
- [noname-oneup-singleton](https://github.com/sajadimn/noname-oneup-singleton)
- [noname-oneup-region](https://github.com/sajadimn/noname-oneup-region)

## How to use
1. Add this package. It creates `AdMobAdsConfig`, `ApplovinAdsConfig` and `TapsellAdsConfig` in `Assets/Settings/Resources/Ads`.
2. The three ad SDKs install automatically. If one fails, for example with no internet, run `OneUp/Ads/Install SDKs` after restarting Unity.
3. Enter your keys in the three config assets. AdMob and AppLovin have separate Android and iOS keys.
4. Call `AdsManager.Instance.Init()`. No prefab or scene object is needed.

Google consent is included. AdMob and AppLovin use it.
