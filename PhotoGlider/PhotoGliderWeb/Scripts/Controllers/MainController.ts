/// <reference path="../../Scripts/typings/angularjs/angular.d.ts" />

module Main {
    export interface Scope {
        images: string[];
        loadMore: () => void;
    }

    export class Controller {
        constructor(private $scope: Scope, $http: ng.IHttpService, $q: ng.IQService) {
            
            this.after = "";
            this.busy = false;

            $scope.loadMore = () => {
                if (this.busy) return;
                this.busy = true;

                var url = "";
                if (this.after) {
                    url = "http://www.reddit.com/r/all/new.json?after=" + this.after
                } else {
                    url = "http://www.reddit.com/r/all/new.json";
                }

                var deferred = $q.defer();

                return $http.get(url)
                    .success((data: any) => {
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
                            $scope.images.push(img);
                        });
                        this.after = data.data.after;
                        this.busy = false;
                    })
                    .error((data: any) => {
                        deferred.reject(data);
                    });
                return deferred.promise;
            }

            $scope.loadMore();
        }

        busy: boolean;
        after: string;
    }
}

angular.module('PhotoGliderWeb', ['infinite-scroll']);