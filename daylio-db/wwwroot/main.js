$(function() {
    console.log('jquery is working!');
    createGraph();
  });

// gridlines in x axis function
function make_x_gridlines() {		
    return d3.axisBottom(x)
        .ticks(5)
}

// gridlines in y axis function
function make_y_gridlines() {		
    return d3.axisLeft(y)
        .ticks(5)
}

// define the line
var valueline = d3.line()
    .x(function(d) { return x(d.date); })
    .y(function(d) { return y(d.count); });

var parseTime = d3.timeParse("%Y-%m")
  
function createGraph() {
    var width = 960; // chart width
    var height = 700; // chart height
    var format = d3.format(",d");  // convert value to integer
    var color = d3.scale.category20();  // create ordinal scale with 20 colors
    var sizeOfRadius = d3.scale.pow().domain([-100,100]).range([-50,50]);

    var bubble = d3.layout.pack()
        .sort(null)  // disable sorting, use DOM tree traversal
        .size([width, height])  // chart layout size
        .padding(1)  // padding between circles
        .radius(function(d) { return 20 + (sizeOfRadius(d) * 30); });

    var svg = d3.select("#chart").append("svg")
        .attr("width", width)
        .attr("height", height)
        .attr("class", "bubble");

    // REQUEST THE DATA
    d3.json("/daylio/activityCount", function(error, activities) 
    {
        // format the data
        activities.forEach(function(d) {
            d.date = parseTime(d.year + "-" + d.month);
        });

        var activity1 = activities.filter(function(d)
        {
            if( d["name"] == "yoga")
            { 
                return d;
            } 
        })

        var activity2 = activities.filter(function(d)
        {
            if( d["name"] == "gaming")
            { 
                return d;
            } 
        })

        var max = Math.max(
            Math.max.apply(Math, activity1.map(x => x.count)), 
            Math.max.apply(Math, activity2.map(x => x.count))) + 1;

        var vis = d3.select("#visualisation"),
        WIDTH = 1000,
        HEIGHT = 500,
        MARGINS = {
            top: 20,
            right: 20,
            bottom: 20,
            left: 50
        },

        xScale = d3.scale.linear().range([MARGINS.left, WIDTH - MARGINS.right]).domain([5, 105]),
        yScale = d3.scale.linear().range([HEIGHT - MARGINS.top, MARGINS.bottom]).domain([0, max]),

        xAxis = d3.svg.axis()
            .scale(xScale),
        
        yAxis = d3.svg.axis()
            .scale(yScale)
            .orient("left");

        // add the X gridlines
        vis.append("g")			
            .attr("class", "grid")
            .attr("transform", "translate(0," + height + ")")
            .call(make_x_gridlines()
                .tickSize(-height)
                .tickFormat("")
            )

        // add the Y gridlines
        vis.append("g")			
            .attr("class", "grid")
            .call(make_y_gridlines()
                .tickSize(-width)
                .tickFormat("")
            )

        // add the valueline path.
        vis.append("path")
            .data([data])
            .attr("class", "line")
            .attr("d", valueline);
    
     
        // X axis
        vis.append("svg:g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + (HEIGHT - MARGINS.bottom) + ")")
            .call(xAxis);

        // Y axis
        vis.append("svg:g")
            .attr("class", "y axis")
            .attr("transform", "translate(" + (MARGINS.left) + ",0)")
            .call(yAxis);       
            
        var lineGen = d3.svg.line()
            .x(function(d) {
                return xScale(date);
            })
            .y(function(d) {
                return yScale(d.count);
            })
            .interpolate("basis");

        vis.append('svg:path')
            .attr('d', lineGen(activity1))
            .attr('stroke', 'green')
            .attr('stroke-width', 2)
            .attr('fill', 'none');

        vis.append('svg:path')
            .attr('d', lineGen(activity2))
            .attr('stroke', 'blue')
            .attr('stroke-width', 2)
            .attr('fill', 'none');

    });
}