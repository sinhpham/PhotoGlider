/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />

module Main {
    export interface Scope {
        images: string[];
        loadMore: () => void;
    }

    export class Controller {
        constructor(private $scope: Scope, private $http: ng.IHttpService, private $q: ng.IQService) {
            $scope.images = ["1", "2", "3"];
            $scope.loadMore = this.loadMore;
            this.after = "";
            var a = 0;
            //this.getInitialPage();
        }

        busy: boolean;
        after: string;

        getInitialPage() {
            var deferred = this.$q.defer();
            var scope = this.$scope;
            var currAfter = this.after;
            ///blah
            return this.$http.get('http://www.reddit.com/r/all/new.json?limit=50')
                .success(function (data: any) {
                    var count = data.data.children.length;
                    deferred.resolve(data);
                    var dataArr = <Array<any>>data.data.children;
                    scope.images = dataArr.filter((v, idx, arr) => {
                        var tLink = v.data.thumbnail;
                        if (tLink == "" || tLink == "self" || tLink == "nsfw" || tLink == "default") {
                            return false;
                        }
                        return true;
                    }).map((v, idx, arr) => {

                            return v.data.thumbnail;
                        });
                    currAfter = data.data.after;
                    var a = 0;
                })
                .error(function (data) {
                    deferred.reject(data);
                });
            return deferred.promise;
        }

        loadMore() {
            if (this.busy) return;
            this.busy = true;
            var url = "";
            if (this.after) {
                url = "http://www.reddit.com/r/all/new.json?after=" + this.after
            } else {
                url = "http://www.reddit.com/r/all/new.json";
            }


            var deferred = this.$q.defer();
            var scope = this.$scope;

            return this.$http.get(url)
                .success(function (data: any) {
                    var count = data.data.children.length;
                    deferred.resolve(data);
                    var dataArr = <Array<any>>data.data.children;
                    var newImages = dataArr.filter((v, idx, arr) => {
                        var tLink = v.data.thumbnail;
                        if (tLink == "" || tLink == "self" || tLink == "nsfw" || tLink == "default") {
                            return false;
                        }
                        return true;
                    }).map((v, idx, arr) => {

                            return v.data.thumbnail;
                        });
                    newImages.forEach(img => {
                        scope.images.push(img);
                    });

                })
                .error(function (data) {
                    deferred.reject(data);
                });
            return deferred.promise;
        }
    }
}

angular.module('PhotoGliderWeb', ['infinite-scroll']);

angular.module('myApp', ['infinite-scroll']);

module myApp {
    export interface Scope {
        reddit: Reddit
    }

    export class Reddit {
        constructor(private $http: ng.IHttpService) {
            this.items = [];
            this.busy = false;
            this.after = '';
        }

        nextPage() {
            if (this.busy) return;
            this.busy = true;

            var url = "http://api.reddit.com/hot?after=" + this.after + "&jsonp=JSON_CALLBACK";
            this.$http.jsonp(url).success(function (data) {
                var items = data.data.children;
                for (var i = 0; i < items.length; i++) {
                    this.items.push(items[i].data);
                }
                this.after = "t3_" + this.items[this.items.length - 1].id;
                this.busy = false;
            }.bind(this));
        }

        items: string[];
        busy: boolean;
        after: string;
    }

    export class DemoController {
        constructor($scope: Scope, $http: ng.IHttpService) {
            $scope.reddit = new Reddit($http);
        }
    }
}
