using System;
using System.Collections.Generic;
using System.Text;

namespace Bluefragments.Utilities.Graph
{
    public class GraphPhotoResponse
    {
        public byte[] Bytes { get; set; }

        public string ContentType { get; set; }

        public string Base64String { get; set; }
    }
}
