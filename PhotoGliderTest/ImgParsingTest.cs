﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoGliderPCL.Models;

namespace PhotoGliderTest
{
    [TestClass]
    public class ImgParsingTest
    {
        [TestMethod]
        public void MobileURL()
        {
            var extractRet = ImgUrlExtractor.Extract("http://m.imgur.com/2r9QSsy").Result;

            Assert.AreEqual(extractRet.Item1.ImageLink, "http://i.imgur.com/2r9QSsy.jpg");
            Assert.AreEqual(extractRet.Item1.ThumbnailLink, "http://i.imgur.com/2r9QSsym.jpg");
        }

        [TestMethod]
        public void DirectImgLink()
        {
            var extractRet = ImgUrlExtractor.Extract("http://i.imgur.com/403hsL9.jpg").Result;

            Assert.AreEqual(extractRet.Item1.ImageLink, "http://i.imgur.com/403hsL9.jpg");
            Assert.AreEqual(extractRet.Item1.ThumbnailLink, "http://i.imgur.com/403hsL9m.jpg");
        }

        [TestMethod]
        public void GalleryImgLink()
        {
            var extractRet = ImgUrlExtractor.Extract("http://imgur.com/gallery/MpoHLv0").Result;

            Assert.AreEqual(extractRet.Item1.ImageLink, "http://imgur.com/gallery/MpoHLv0");
            Assert.AreEqual(extractRet.Item1.ThumbnailLink, null);
        }

        [TestMethod]
        public void AlbumImgLink()
        {
            var res = ImgUrlExtractor.Extract("http://imgur.com/a/4zD5s").Result;
            Assert.AreNotEqual(res.Item2, null);
        }
    }
}
