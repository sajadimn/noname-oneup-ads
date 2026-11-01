#if ONEUP_ADMOB
using System;
using UnityEngine;
using GoogleMobileAds.Ump.Api;

public class GoogleConsentManager : MonoSingleton<GoogleConsentManager>
{
    private bool consentState;

    public void InitializeConsent()
    {
        ConsentRequestParameters requestParameters = new ConsentRequestParameters();
        ConsentInformation.Update(requestParameters, OnConsentInfoUpdated);
    }

    private void OnConsentInfoUpdated(FormError error)
    {
        if (error != null)
        {
            Debug.LogError("GoogleConsentManager - UMP Update Error: " + error.Message);
            OnDoneConsent();
            return;
        }

        Debug.Log("GoogleConsentManager - UMP Consent Status: " + ConsentInformation.ConsentStatus);
        ConsentForm.LoadAndShowConsentFormIfRequired(OnConsentFormDismissed);
    }

    private void OnConsentFormDismissed(FormError error)
    {
        if (error != null)
        {
            Debug.LogError("GoogleConsentManager - UMP Consent Form Error: " + error.Message);
        }

        OnDoneConsent();
    }

    private void OnDoneConsent()
    {
        if (consentState)
            return;

        if (!ConsentInformation.CanRequestAds())
        {
            Debug.Log("GoogleConsentManager - UMP: Ads cannot be requested.");
            return;
        }

        consentState = true;
        Debug.Log("GoogleConsentManager - UMP: Ads can be requested.");
        AdsManager.Instance.GetCurrentAds()?.InitializeAds();
    }

    public void ShowPrivacyOptions()
    {
        if (ConsentInformation.PrivacyOptionsRequirementStatus != PrivacyOptionsRequirementStatus.Required)
        {
            Debug.Log("GoogleConsentManager - UMP: Privacy options are not required.");
            return;
        }

        ConsentForm.ShowPrivacyOptionsForm((FormError error) =>
            {
                if (error != null)
                {
                    Debug.LogError("GoogleConsentManager - Privacy Options Error: " + error.Message);
                }
            }
        );
    }
}
#else
using UnityEngine;

public class GoogleConsentManager : MonoSingleton<GoogleConsentManager>
{
    public void InitializeConsent()
    {
        Debug.LogWarning("GoogleConsentManager - Google Mobile Ads is not installed, skipping consent.");
        AdsManager.Instance.GetCurrentAds()?.InitializeAds();
    }

    public void ShowPrivacyOptions()
    {
    }
}
#endif
