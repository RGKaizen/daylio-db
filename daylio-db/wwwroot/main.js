var groupBy = function(xs, key) {
    return xs.reduce(function(rv, x) {
      (rv[x[key]] = rv[x[key]] || []).push(x);
      return rv;
    }, {});
  };

$(function() {
    console.log('jquery is working!');
    createGraph();
  });
  
function createGraph() {
    var width = 960; // chart width
    var height = 700; // chart height
    var format = d3.format(",d");  // convert value to integer
    var color = d3.scale.category20();  // create ordinal scale with 20 colors
    var sizeOfRadius = d3.scale.pow().domain([-100,100]).range([-50,50]);

    var svg = d3.select("#chart").append("svg")
        .attr("width", width)
        .attr("height", height);

    // REQUEST THE DATA
    d3.json("/daylio/activityCount", function(error, activities) 
    {

        activities = activities.filter(function(d) 
        {
            if(
               d["name"] == "" 
            || d["name"] == "" 
            || d["name"] == "work"
            || d["name"] == "cooked"
            || d["name"] == "gaming"
            )
            {
                return d;
            }

        })

        //ar parseTime = d3.timeParse("%Y/%m");
        var format = d3.time.format("%Y-%m");
        // format the data
        activities.forEach(function(d) {
            d.date = format.parse(d.year + "-" + d.month);
        });

        var max = Math.max.apply(Math, activities.map(x => x.count)) + 1; 

        // define the x scale (horizontal)
        var mindate = new Date(2017,4,15),
        maxdate = new Date(2018,3,0);

        var vis = d3.select("#visualisation"),
        WIDTH = 1000,
        HEIGHT = 500,
        MARGINS = {
            top: 20,
            right: 20,
            bottom: 20,
            left: 50
        },
        xScale = d3.time.scale().range([MARGINS.left, WIDTH - MARGINS.right]).domain([mindate, maxdate]),
        yScale = d3.scale.linear().range([HEIGHT - MARGINS.top, MARGINS.bottom]).domain([0, max]),

        xAxis = d3.svg.axis()
            .scale(xScale),
        
        yAxis = d3.svg.axis()
            .scale(yScale)
            .orient("left");
     
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
    
        // Function for determining x and y values for lines
        var lineGen = d3.svg.line()
            .x(function(d) {
                return xScale(d.date);
            })
            .y(function(d) {
                return yScale(d.count);
            })
            .interpolate("basis");

        var groupedActivities = groupBy(activities, "name");

        var color = d3.scale.category10()
        var i = 0;
        for(var key in groupedActivities)
        {
            vis.append('svg:path')
                .attr('d', lineGen(groupedActivities[key]))
                .attr('stroke', color(i))
                .attr('stroke-width', 2)
                .attr('fill', 'none')
                .attr('class', 'line')
            vis.append("text")
                .attr("transform", "translate(" + ((width+3)-100) + "," + ((30*i) + 200) + ")")
                .attr("dy", ".35em")
                .attr("text-anchor", "start")
                .style("fill", color(i))
                .text(key);   
            i++;      
        }
    
    });
}