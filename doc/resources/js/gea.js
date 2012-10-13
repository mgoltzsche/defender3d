var gea = {
	viewBase: 'views/',
	navigationModel: [
		{id: 'welcome', title: 'Willkommen'},
		{id: 'tasks', title: 'Aufgaben', children: [
			{id: 'idea', title: 'Idee'},
			{id: 'concept', title: 'Konzept'},
			{id: 'prototype', title: 'Prototyp'},
			{id: 'game', title: 'Spiel'}
		]}
	],
	
	NavigationController: function($scope) {
		$scope.navigationModel = gea.navigationModel
	},
	
	ContentController: function() {
	},
	
	configureAnchor2ViewMapping: function(routeProvider, viewConfigurations) {
		for (var i = 0; i < viewConfigurations.length; i++) {
			var cfg = viewConfigurations[i];
			routeProvider.when('/' + cfg.id, {templateUrl: gea.viewBase + cfg.id + '.html', controller: gea.ContentController});
			
			if (cfg.children) {
				this.configureAnchor2ViewMapping(routeProvider, cfg.children);
			}
		}
	}
};

// configure url -> view mapping
angular.module('gea', []).config(['$routeProvider', function($routeProvider) {
	gea.configureAnchor2ViewMapping($routeProvider, gea.navigationModel);
	
	if (gea.navigationModel.length) {
		$routeProvider.otherwise({redirectTo: '/' + gea.navigationModel[0].id});
	}
}]);
