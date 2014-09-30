/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />

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
                    url = "http://www.reddit.com/r/all/new.json?after=" + this.after
                } else {
                    url = "http://www.reddit.com/r/all/new.json";
                }

                $http.get(url)
                    .success((data:any) => {
                        var count = data.data.children.length;
                        //deferred.resolve(data);
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


                            return ri;
                        });

                        newImages.forEach(img => {
                            $scope.images.push(img);
                        });
                        this.after = data.data.after;
                        this.busy = false;
                    })
                    .error((data:any) => {
                        //deferred.reject(data);
                    });
            };

            $scope.loadMore();
        }

        busy:boolean;
        after:string;
    }
}

angular.module('PhotoGliderWeb', ['infinite-scroll']);