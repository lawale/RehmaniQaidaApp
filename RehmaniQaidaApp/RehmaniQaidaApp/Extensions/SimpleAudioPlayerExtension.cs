using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace RehmaniQaidaApp.Extensions
{
    public static class SimpleAudioPlayerExtension
    {
        public static void Reset(this ISimpleAudioPlayer player, Stream stream)
        {
            //player?.Dispose();
            player = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            stream.Position = 0;
            player?.Load(stream);
        }

        public static void Reset(this List<ISimpleAudioPlayer> audioPlayers, ObservableCollection<Stream> streams)
        {
            for(var index = 0; index < audioPlayers.Count(); ++index)
            {
                audioPlayers[index].Reset(streams[index]);
            }
        }
    }
}
