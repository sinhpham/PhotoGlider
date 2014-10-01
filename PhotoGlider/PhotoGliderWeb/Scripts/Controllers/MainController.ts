/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />
/// <reference path="../../Scripts/typings/urijs/URI.d.ts" />

module Main {

    export class InternetImage {
        ImageLink:string;
        ThumbnailLink:string;
    }

    export class RedditImage {

        DisplayingImage:InternetImage;

        Permalink:string;
        OriginalUrl:string;
        NSFW:boolean;

        GalleryImages:InternetImage[];
    }

    export interface Scope {
        images: RedditImage[];
        loadMore: () => void;
    }

    export class Controller {
        constructor(private $scope:Scope, $http:ng.IHttpService, $q:ng.IQService) {

            this.after = "";
            this.busy = false;
            this.$scope.images = [];


            $scope.loadMore = () => {
                if (this.busy) return;
                this.busy = true;

                var url = "";
                if (this.after) {
                    url = "http://www.reddit.com/r/pics/new.json?after=" + this.after
                } else {
                    url = "http://www.reddit.com/r/pics/new.json";
                }

                $http.get(url)
                    .success((data:any) => {
                        var count = data.data.children.length;
                        var dataArr = <Array<any>>data.data.children;
                        var newImages = dataArr.filter((v, idx, arr) => {
                            var tLink = v.data.thumbnail;
                            if (tLink == "" || tLink == "self" || tLink == "nsfw" || tLink == "default") {
                                return false;
                            }
                            return true;
                        }).map((v, idx, arr) => {
                            var ri = new RedditImage();
                            ri.OriginalUrl = v.data.url;
                            ri.Permalink = v.data.permalink;
                            ri.NSFW = v.data.over_18;
                            ri.DisplayingImage = new InternetImage();
                            ri.DisplayingImage.ImageLink = v.data.url;
                            ri.DisplayingImage.ThumbnailLink = v.data.thumbnail;

                            var ext = new ImgUrlExtractor($http);
                            var ans = ext.Extract(ri.OriginalUrl);


                            return ri;
                        });

                        newImages.forEach(img => {
                            $scope.images.push(img);
                        });
                        this.after = data.data.after;
                        this.busy = false;
                    })
                    .error((data:any) => {
                    });
            };

            $scope.loadMore();
        }

        busy:boolean;
        after:string;
    }

    /*export interface Dictionary<TKey, TValue> {
     [index: TKey]: TValue;
     }*/

    export class ImgUrlExtractor {
        constructor(private $http:ng.IHttpService) {
        }

        Extract(inputUrl:string):{} {
            var ret = new InternetImage();
            ret.ImageLink = inputUrl;
            var galleryUrls:InternetImage[] = [];

            var u = new URI(inputUrl);
            if (u.host() == "m.imgur.com") {
                // Turn mobile page into normal page.
                u.host("imgur.com");
            }

            if (u.host() == "imgur.com") {
                if (u.path().indexOf("/a") == 0) {
                    // Imgur albums.
                    galleryUrls = new Array<InternetImage>();
                    ret = null;

                    var apArr = u.path().split("/").filter(currStr => currStr != "");

                    var albumBlogLayoutUrl = "http://" + u.host() + "/" + apArr[0] + "/" + apArr[1] + "/layout/blog";

                    this.$http.get(albumBlogLayoutUrl)
                        .success((data:any) => {
                            var parser = new DOMParser();
                            var htmlDoc = parser.parseFromString(data, "text/html");


                        });

                    /*var albumPage: string = null;

                     if (albumPage != null) {
                     var htmlDoc = new HtmlDocument();
                     htmlDoc.LoadHtml(albumPage);
                     if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0 && htmlDoc.DocumentNode != null) {
                     var imgDivs = htmlDoc.DocumentNode.Descendants().Where(n => n.Name == "div" && n.Attributes.FirstOrDefault(a => a.Name == "class" && a.Value == "image") != null);
                     galleryUrls = imgDivs.Select(iDiv => {
                     var imgNode = iDiv.Descendants().Where(n => n.Name == "img");
                     if (imgNode.Count() != 1) {
                     Debug.WriteLine("Error in parsing");
                     }

                     var imgLink = string.Format("http:{0}", imgNode.First().GetAttributeValue("data-src", "not found"));

                     var retii = new InternetImage();
                     retii.ImageLink = imgLink;
                     retii.ThumbnailLink = this.GetThumbnailPathFromUrl(imgLink);
                     return retii;
                     }).ToList();
                     }
                     }*/
                }
                else if (u.path().indexOf("/gallery/") == 0) {
                    // TODO: parse gallery links.
                    var a = 0;
                }
                else {
                    // Imgur single image page.
                    var imgLink = "http://i.imgur.com" + u.path();
                    if (u.path().indexOf('.') == -1) {
                        imgLink += ".jpg";
                    }
                    ret.ImageLink = imgLink;
                    ret.ThumbnailLink = this.GetThumbnailPathFromUrl(imgLink);
                }
            }
            else if (u.host() == "i.imgur.com") {
                // Direct link to an image.
                ret.ThumbnailLink = this.GetThumbnailPathFromUrl(ret.ImageLink);
            }

            var map:{ [key: string]: any; } = { };
            map["internetimage"] = ret;
            map["gallery"] = galleryUrls;

            return map;
        }

        private GetThumbnailPathFromUrl(inputUrl:string):string {
            var ret:string = null;

            var u = new URI(inputUrl);
            if (u.host() == "i.imgur.com") {
                var strArr = u.path().split('.');
                if (strArr.length == 2) {
                    ret = "http://" + u.host() + strArr[0] + "m." + strArr[1];
                }
            }

            return ret;
        }
    }
}

angular.module('PhotoGliderWeb', ['infinite-scroll']);