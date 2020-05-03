using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

public class GoogleAdsManager : SingletonPattern<GoogleAdsManager>
{
    // Admob id
    public const string bannerId = "ca-app-pub-2578695429525909/7490615939";
    public const string interstitialId = "ca-app-pub-2578695429525909/1576137797";
    
    // Test id
    public const string bannerId_Test = "ca-app-pub-3940256099942544/6300978111";
    public const string interstitialId_Test = "ca-app-pub-3940256099942544/1033173712";
        
    private BannerView bannerView;
    private InterstitialAd interstitial;

    public InterstitialAd GetInterstitial()
    {
        return interstitial;
    }

    protected override void Init()
    {
        base.Init();
        Debug.Log("GoogleAdsManager Init");
    }

    public void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = bannerId;
#elif UNITY_IPHONE
        string adUnitId = bannerId;
#else
        string adUnitId = "unexpected_platform";
#endif
        Debug.Log("Request banner");
        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        bannerView.LoadAd(request);
        bannerView.OnAdLoaded += (sender, args) =>
        {
            Debug.Log("banner loaded");
            bannerView.Show();
        };
    }

    public void ToggleBanner(bool visibility)
    {
        if (visibility) bannerView.Show();
        else bannerView.Hide();
    }

    public void DestroyBanner()
    {
        bannerView.Destroy();
    }

    public void ShowBanner()
    {
        Debug.Log("Show banner");
        bannerView.Show();
    }

    public void RequestInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = interstitialId;
#elif UNITY_IPHONE
        string adUnitId = interstitialId;
#else
        string adUnitId = "unexpected_platform";
#endif
        Debug.Log("Requewst interstitial");
        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);
        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    public void HandleOnAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("Interstitial OnAdLoaded");
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log("Interstitial HandleOnAdFailedToLoad message: "
                  + args.Message);
    }

    public void HandleOnAdOpened(object sender, EventArgs args)
    {
        Debug.Log("Interstitial OnAdOpened");
    }

    public void HandleOnAdClosed(object sender, EventArgs args)
    {
        Debug.Log("Interstitial OnAdClosed");
        RequestInterstitial();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {
        Debug.Log("Interstitial LeavingApplication");
    }

    public void ShowInterstitial()
    {
        if (interstitial != null && interstitial.IsLoaded())
        {
            Debug.Log($"ShowInterstitial Show");
            interstitial.Show();
        }
        else
        {
            Debug.Log($"ShowInterstitial Request");
            RequestInterstitial();
            StartCoroutine(ShowInterstitalAsync());
        }
    }

    IEnumerator ShowInterstitalAsync()
    {
        while (!interstitial.IsLoaded())
        {
            yield return null;
        }

        interstitial.Show();
    }
}