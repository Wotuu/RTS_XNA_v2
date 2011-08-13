using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketLibrary.Protocol
{
    public class TestHeaders
    {
        // 0 - 31   Chat reserved ranges (0x00, 0x1F )
        // 32 - 47  General game reserved ranges (0x20, 0x2F)
        // 48 - 63  Unit reserved ranges (0x30, 0x3F)
        // 64 - 79  Building reserved ranges (0x40, 0x4F)
        // 80 - 96  Test reserved ranges (0x50, 0x5F);

        /// <summary>
        /// Used for steady testing
        /// [Header]
        /// </summary>
        public const int STEADY_TEST = 0x50;

        /// <summary>
        /// Used for burst testing
        /// [Header]
        /// </summary>
        public const int BURST_TEST = 0x51;

        /// <summary>
        /// Used for malform testing
        /// [Header] [String randomString]
        /// </summary>
        public const int MALFORM_TEST = 0x52;
    }
}
