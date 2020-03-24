﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using XboxGameClipLibrary.API;
using XboxGameClipLibrary.Models;
using XboxGameClipLibrary.Models.Screenshots;
using XboxGameClipLibrary.ViewModels.CapturesPage;

namespace XboxGameClipLibrary.Views
{
    public partial class CapturesPage : Page, IProvideCustomContentState
    {
        public CapturesPage()
        {
            // Create a CancellationTokenSource object
            CancellationTokenSource cts = new CancellationTokenSource();

            var cvm = new CapturesViewModel
            {
                GameClips = Task.Run(() => GetGameClips(cts.Token)).Result,
                Screenshots = Task.Run(() => GetScreenshots(cts.Token)).Result
            };

            // Request cancellation.
            cts.Cancel();

            // Cancellation should have happened, so call Dispose.
            cts.Dispose();

            // Bind the CapturesViewModel to DataContext
            DataContext = cvm;
        }

        public CustomContentState GetContentState()
        {
            return new RestoreModelContentState(DataContext);
        }

        private async Task<List<Screenshot>> GetScreenshots(CancellationToken token)
        {
            var xuid = await GetXuid(token);
            var screenshots = Task.Run(() => Api.GetScreenshotsFromStreamCallAsync(token, xuid)).Result;

            // Debug Screenshot response
            string jsonString = JsonConvert.SerializeObject(screenshots, Formatting.Indented);
            Console.WriteLine(screenshots);

            return screenshots;
        }

        private async Task<List<GameClip>> GetGameClips(CancellationToken token)
        {
            var xuid = await GetXuid(token);
            var gameClips = Task.Run(() => Api.GetGameClipsFromStreamCallAsync(token, xuid)).Result;

            // Debug GameClip response
            string jsonString = JsonConvert.SerializeObject(gameClips, Formatting.Indented);
            Console.WriteLine(jsonString);

            return gameClips;
        }

        private async Task<string> GetXuid(CancellationToken token)
        {
            var profile = await Task.Run(() => Api.GetProfileFromStringCallAsync(token));
            var xuid = profile["userXuid"].ToString();

            // Debug Profile response
            Console.WriteLine(profile);

            // Debug Xuid response
            Console.WriteLine("Xuid: " + xuid);

            return xuid;
        }
    }

    [Serializable]
    internal class RestoreModelContentState : CustomContentState
    {
        private readonly object _model;

        public RestoreModelContentState(object model)
        {
            _model = model;
        }

        public override void Replay(NavigationService navigationService, NavigationMode mode)
        {
            var element = navigationService.Content as FrameworkElement;
            if (element == null) return;
            element.DataContext = _model;
        }
    }
}