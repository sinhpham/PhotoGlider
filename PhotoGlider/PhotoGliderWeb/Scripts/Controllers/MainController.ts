/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />

module Main {
    export interface Scope {
        images: string[]
    }

    export class Controller {
        constructor(private $scope: Scope, private $http: ng.IHttpService, private $q: ng.IQService) {
            $scope.images = ["1", "2", "3"];
            this.getInitialPage();
        }

        getInitialPage() {
            var deferred = this.$q.defer();
            var scope = this.$scope;
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
                    console.log(data);
                })
                .error(function (data) {
                    deferred.reject(data);
                });
            return deferred.promise;
        }

        //loadMore() {
        //    var last = this.$scope.images[this.$scope.images.length - 1];
        //    for (var i = 1; i <= 8; i++) {
        //        this.$scope.images.push(last + i);
        //    }
        //    var a = 0;
        //}
    }
}

angular.module('PhotoGliderWeb', []);
//angular.module('', ['infinite-scroll']);