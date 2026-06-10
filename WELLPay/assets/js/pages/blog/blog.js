$(function () {
    "use strict";    
    initCounters();
    getMorris('line', 'line_chart');
});

function getMorris(type, element) {
    
    if (type === 'line') {
        Morris.Line({
            element: element,
            data: [
                {
                    period: '2012',
                    WebDesign: 120,
                    Photography: 610,
                    Technology: 301,
                    Lifestyle: 250,
                    Sports: 150
                },{
                    period: '2013',
                    WebDesign: 340,
                    Photography: 201,
                    Technology: 150,
                    Lifestyle: 214,
                    Sports: 250
                },{
                    period: '2014',
                    WebDesign: 458,
                    Photography: 450,
                    Technology: 501,
                    Lifestyle: 410,
                    Sports: 400
                }, {
                    period: '2015',
                    WebDesign: 265,
                    Photography: 521,
                    Technology: 265,
                    Lifestyle: 98,
                    Sports: 436
                }, {
                    period: '2016',
                    WebDesign: 200,
                    Photography: 215,
                    Technology: 280,
                    Lifestyle: 300,
                    Sports: 249
                },
                {
                    period: '2017',
                    WebDesign: 865,
                    Photography: 450,
                    Technology: 501,
                    Lifestyle: 410,
                    Sports: 400
                }
            ],
            xkey: 'period',
            ykeys: ['WebDesign', 'Photography', 'Technology', 'Lifestyle', 'Sports'],
            labels: ['WebDesign', 'Photography', 'Technology', 'Lifestyle', 'Sports'],
            pointSize: 2,
            fillOpacity: 0,
            pointStrokeColors: ['#fe6283', '#359ef0', '#fcce56', '#48c2c2', '#9d67ff'],
            behaveLikeLine: true,
            gridLineColor: '#515151',
            lineWidth: 1,
            hideHover: 'auto',
            lineColors: ['#fe6283', '#359ef0', '#fcce56', '#48c2c2', '#9d67ff'],
            resize: true
        });
    }
}
//====
$(function() {
	"use strict";
	var mapData = {
			"US": 298,
			"SA": 200,
			"AU": 760,
			"IN": 2000000,
			"GB": 120,
		};
	
	if( $('#world-map-markers').length > 0 ){
		$('#world-map-markers').vectorMap(
		{
			map: 'world_mill_en',
			backgroundColor: 'transparent',
			borderColor: '#fff',
			borderOpacity: 0.25,
			borderWidth: 0,
			color: '#e6e6e6',
			regionStyle : {
				initial : {
				  fill : '#f4f4f4'
				}
			  },

			markerStyle: {
			  initial: {
							r: 5,
							'fill': '#fff',
							'fill-opacity':1,
							'stroke': '#000',
							'stroke-width' : 1,
							'stroke-opacity': 0.4
						},
				},
		   
			markers : [{
				latLng : [21.00, 78.00],
				name : 'INDIA : 350'
			  
			  },
			  {
				latLng : [-33.00, 151.00],
				name : 'Australia : 250'
				
			  },
			  {
				latLng : [36.77, -119.41],
				name : 'USA : 250'
			  
			  },
			  {
				latLng : [55.37, -3.41],
				name : 'UK   : 250'
			  
			  },
			  {
				latLng : [25.20, 55.27],
				name : 'UAE : 250'
			  
			  }],

			series: {
				regions: [{
					values: {
						"US": '#49c5b6',
						"SA": '#667add',
						"AU": '#50d38a',
						"IN": '#60bafd',
						"GB": '#ff758e',
					},
					attribute: 'fill'
				}]
			},
			hoverOpacity: null,
			normalizeFunction: 'linear',
			zoomOnScroll: false,
			scaleColors: ['#000000', '#000000'],
			selectedColor: '#000000',
			selectedRegions: [],
			enableZoom: false,
			hoverColor: '#fff',
		});
	}	

	if( $('#usa').length > 0 ){
		$('#usa').vectorMap({
			map : 'us_aea_en',
			backgroundColor : 'transparent',
			regionStyle : {
				initial : {
					fill : '#00c5dc'
				}
			}
		});
	} 
});
