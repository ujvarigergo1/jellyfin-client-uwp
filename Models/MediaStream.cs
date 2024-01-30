using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Models
{
    public class MediaStream
    {
        private static readonly string[] _specialCodes =
        {
            // Uncoded languages.
            "mis",
            // Multiple languages.
            "mul",
            // Undetermined.
            "und",
            // No linguistic content; not applicable.
            "zxx"
        };

        /// <summary>
        /// Gets or sets the codec.
        /// </summary>
        /// <value>The codec.</value>
        public string Codec { get; set; }

        /// <summary>
        /// Gets or sets the codec tag.
        /// </summary>
        /// <value>The codec tag.</value>
        public string CodecTag { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the color range.
        /// </summary>
        /// <value>The color range.</value>
        public string ColorRange { get; set; }

        /// <summary>
        /// Gets or sets the color space.
        /// </summary>
        /// <value>The color space.</value>
        public string ColorSpace { get; set; }

        /// <summary>
        /// Gets or sets the color transfer.
        /// </summary>
        /// <value>The color transfer.</value>
        public string ColorTransfer { get; set; }

        /// <summary>
        /// Gets or sets the color primaries.
        /// </summary>
        /// <value>The color primaries.</value>
        public string ColorPrimaries { get; set; }

        /// <summary>
        /// Gets or sets the Dolby Vision version major.
        /// </summary>
        /// <value>The Dolby Vision version major.</value>
        public int? DvVersionMajor { get; set; }

        /// <summary>
        /// Gets or sets the Dolby Vision version minor.
        /// </summary>
        /// <value>The Dolby Vision version minor.</value>
        public int? DvVersionMinor { get; set; }

        /// <summary>
        /// Gets or sets the Dolby Vision profile.
        /// </summary>
        /// <value>The Dolby Vision profile.</value>
        public int? DvProfile { get; set; }

        /// <summary>
        /// Gets or sets the Dolby Vision level.
        /// </summary>
        /// <value>The Dolby Vision level.</value>
        public int? DvLevel { get; set; }

        /// <summary>
        /// Gets or sets the Dolby Vision rpu present flag.
        /// </summary>
        /// <value>The Dolby Vision rpu present flag.</value>
        public int? RpuPresentFlag { get; set; }

        /// <summary>
        /// Gets or sets the Dolby Vision el present flag.
        /// </summary>
        /// <value>The Dolby Vision el present flag.</value>
        public int? ElPresentFlag { get; set; }

        /// <summary>
        /// Gets or sets the Dolby Vision bl present flag.
        /// </summary>
        /// <value>The Dolby Vision bl present flag.</value>
        public int? BlPresentFlag { get; set; }

        /// <summary>
        /// Gets or sets the Dolby Vision bl signal compatibility id.
        /// </summary>
        /// <value>The Dolby Vision bl signal compatibility id.</value>
        public int? DvBlSignalCompatibilityId { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The comment.</value>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the time base.
        /// </summary>
        /// <value>The time base.</value>
        public string TimeBase { get; set; }

        /// <summary>
        /// Gets or sets the codec time base.
        /// </summary>
        /// <value>The codec time base.</value>
        public string CodecTimeBase { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }



        /// <summary>
        /// Gets the video dovi title.
        /// </summary>
        /// <value>The video dovi title.</value>
        public string VideoDoViTitle
        {
            get
            {
                var dvProfile = DvProfile;
                var rpuPresentFlag = RpuPresentFlag == 1;
                var blPresentFlag = BlPresentFlag == 1;
                var dvBlCompatId = DvBlSignalCompatibilityId;

                if (rpuPresentFlag
                    && blPresentFlag
                    && (dvProfile == 4
                        || dvProfile == 5
                        || dvProfile == 7
                        || dvProfile == 8
                        || dvProfile == 9))
                {
                    var title = "DV Profile " + dvProfile;

                    if (dvBlCompatId > 0)
                    {
                        title += "." + dvBlCompatId;
                    }

                    if(dvBlCompatId == 1)
                    {
                        return title + " (HDR10)";
                    } else if(dvBlCompatId == 2)
                    {
                        return title + " (SDR)";
                    } else if(dvBlCompatId == 4)
                    {
                        return title + " (HLG)";
                    } else {
                        return title;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the audio spatial format.
        /// </summary>
        /// <value>The audio spatial format.</value>
        [DefaultValue(AudioSpatialFormat.None)]
        public AudioSpatialFormat AudioSpatialFormat
        {
            get
            {
                if (Type != MediaStreamType.Audio || string.IsNullOrEmpty(Profile))
                {
                    return AudioSpatialFormat.None;
                }

                return
                    Profile.Contains("Dolby Atmos") ? AudioSpatialFormat.DolbyAtmos :
                    Profile.Contains("DTS:X") ? AudioSpatialFormat.DTSX :
                    AudioSpatialFormat.None;
            }
        }

        public string LocalizedUndefined { get; set; }

        public string LocalizedDefault { get; set; }

        public string LocalizedForced { get; set; }

        public string LocalizedExternal { get; set; }

        public string LocalizedHearingImpaired { get; set; }

       

        public string NalLengthSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is interlaced.
        /// </summary>
        /// <value><c>true</c> if this instance is interlaced; otherwise, <c>false</c>.</value>
        public bool IsInterlaced { get; set; }

        public bool? IsAVC { get; set; }

        /// <summary>
        /// Gets or sets the channel layout.
        /// </summary>
        /// <value>The channel layout.</value>
        public string ChannelLayout { get; set; }

        /// <summary>
        /// Gets or sets the bit rate.
        /// </summary>
        /// <value>The bit rate.</value>
        public int? BitRate { get; set; }

        /// <summary>
        /// Gets or sets the bit depth.
        /// </summary>
        /// <value>The bit depth.</value>
        public int? BitDepth { get; set; }

        /// <summary>
        /// Gets or sets the reference frames.
        /// </summary>
        /// <value>The reference frames.</value>
        public int? RefFrames { get; set; }

        /// <summary>
        /// Gets or sets the length of the packet.
        /// </summary>
        /// <value>The length of the packet.</value>
        public int? PacketLength { get; set; }

        /// <summary>
        /// Gets or sets the channels.
        /// </summary>
        /// <value>The channels.</value>
        public int? Channels { get; set; }

        /// <summary>
        /// Gets or sets the sample rate.
        /// </summary>
        /// <value>The sample rate.</value>
        public int? SampleRate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// </summary>
        /// <value><c>true</c> if this instance is default; otherwise, <c>false</c>.</value>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is forced.
        /// </summary>
        /// <value><c>true</c> if this instance is forced; otherwise, <c>false</c>.</value>
        public bool IsForced { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is for the hearing impaired.
        /// </summary>
        /// <value><c>true</c> if this instance is for the hearing impaired; otherwise, <c>false</c>.</value>
        public bool IsHearingImpaired { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int? Height { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int? Width { get; set; }

        /// <summary>
        /// Gets or sets the average frame rate.
        /// </summary>
        /// <value>The average frame rate.</value>
        public float? AverageFrameRate { get; set; }

        /// <summary>
        /// Gets or sets the real frame rate.
        /// </summary>
        /// <value>The real frame rate.</value>
        public float? RealFrameRate { get; set; }

        /// <summary>
        /// Gets or sets the profile.
        /// </summary>
        /// <value>The profile.</value>
        public string Profile { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public MediaStreamType Type { get; set; }

        /// <summary>
        /// Gets or sets the aspect ratio.
        /// </summary>
        /// <value>The aspect ratio.</value>
        public string AspectRatio { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>The score.</value>
        public int? Score { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is external.
        /// </summary>
        /// <value><c>true</c> if this instance is external; otherwise, <c>false</c>.</value>
        public bool IsExternal { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>The method.</value>
        public SubtitleDeliveryMethod? DeliveryMethod { get; set; }

        /// <summary>
        /// Gets or sets the delivery URL.
        /// </summary>
        /// <value>The delivery URL.</value>
        public string DeliveryUrl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is external URL.
        /// </summary>
        /// <value><c>null</c> if [is external URL] contains no value, <c>true</c> if [is external URL]; otherwise, <c>false</c>.</value>
        public bool? IsExternalUrl { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether [supports external stream].
        /// </summary>
        /// <value><c>true</c> if [supports external stream]; otherwise, <c>false</c>.</value>
        public bool SupportsExternalStream { get; set; }

        /// <summary>
        /// Gets or sets the filename.
        /// </summary>
        /// <value>The filename.</value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the pixel format.
        /// </summary>
        /// <value>The pixel format.</value>
        public string PixelFormat { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        public double? Level { get; set; }

        /// <summary>
        /// Gets or sets whether this instance is anamorphic.
        /// </summary>
        /// <value><c>true</c> if this instance is anamorphic; otherwise, <c>false</c>.</value>
        public bool? IsAnamorphic { get; set; }

       

        public bool SupportsSubtitleConversionTo(string toCodec)
        {

            var fromCodec = Codec;

            // Can't convert from this
            if (string.Equals(fromCodec, "ass", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (string.Equals(fromCodec, "ssa", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Can't convert to this
            if (string.Equals(toCodec, "ass", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (string.Equals(toCodec, "ssa", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        
    }
}
