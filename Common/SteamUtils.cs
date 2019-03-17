using Steamworks;
using UnityEngine;

namespace AlexejheroYTB.Common
{
    public static class SteamUtils
    {
        public static Texture2D GetAvatar(CSteamID user = default(CSteamID))
        {
            user = user == default(CSteamID) ? SteamUser.GetSteamID() : user;
            int FriendAvatar = SteamFriends.GetSmallFriendAvatar(user);
            bool success = Steamworks.SteamUtils.GetImageSize(FriendAvatar, out uint ImageWidth, out uint ImageHeight);

            if (success && ImageWidth > 0 && ImageHeight > 0)
            {
                byte[] Image = new byte[ImageWidth * ImageHeight * 4];
                Texture2D returnTexture = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
                success = Steamworks.SteamUtils.GetImageRGBA(FriendAvatar, Image, (int)(ImageWidth * ImageHeight * 4));
                if (success)
                {
                    returnTexture.LoadRawTextureData(Image);
                    returnTexture.Apply();
                }
                return returnTexture;
            }
            else
            {
                return new Texture2D(0, 0);
            }
        }
    }
}
