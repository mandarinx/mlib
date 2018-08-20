using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.Rendering;

namespace Mlib.System {

    public static partial class Sys {

        // Make minimum system requirements configurable?

#if DEBUG
        public static bool emulateLowEndDevice;
#endif

        private static bool isLowEndDeviceChecked;
        private static bool isLowEndDevice;
        private const uint  memorySizeCap = 3000;

        public static bool IsLowEndDevice() {
#if DEBUG
            if (emulateLowEndDevice) {
                return true;
            }
#endif

            if (isLowEndDeviceChecked) {
                return isLowEndDevice;
            }

            isLowEndDevice = RemoteSettings.GetBool("IsLowEndDevice", GetIsLowEndDevice());
            isLowEndDeviceChecked = true;
            return isLowEndDevice;
        }

        private static bool GetIsLowEndDevice() {
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                return IsLowEndiOS();
            }

            if (!SystemInfo.supportsImageEffects) {
                return true;
            }

            // https://github.com/Unity-Technologies/PostProcessing/issues/130
            if (!SystemInfo.SupportsTextureFormat(TextureFormat.RGBAHalf)) {
                return true;
            }

            //https://docs.unity3d.com/ScriptReference/SystemInfo-graphicsShaderLevel.html
            //30 Shader Model 3, requried for User Lut post processing
            //25 Shader Model 2.5 (DX11 feature level 9.3 feature set)
            //20 Shader Model 2.0.
            if (SystemInfo.graphicsShaderLevel < 30) {
                return true;
            }

            if (SystemInfo.systemMemorySize < memorySizeCap) {
                return true;
            }

            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2) {
                return true;
            }

            return false;
        }

        private static bool IsLowEndiOS() {
            DeviceGeneration device = Device.generation;
            // Devices older than iPad Mini 1st gen, except iPhone 5 are
            // considered low end.
            // See the full list of iOS devices here:
            // https://github.com/Unity-Technologies/UnityCsReference/blob/3cfc6c4729d5cacedf67a38df5de1bfffb5994a3/Runtime/Export/iOS/iOSDevice.bindings.cs
            return device != DeviceGeneration.iPhone5 && (int)device <= 15;
        }
    }
}
