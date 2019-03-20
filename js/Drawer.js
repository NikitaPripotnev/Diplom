
class Drawer{
    constructor(){
        this.canvas = d3.select('svg');
    };
    drawRoute = (routes, palette) => {

        // const drawLine = svg.line()
        //     .x(d => d.x)
        //     .y(d => d.y)
        //     .interpolate('linear');
        console.log(routes, "routes");
        routes.forEach((route, index) => {
            const wrappedRoute = [app.warehouse].concat(route, app.warehouse);
            let d = `M${app.warehouse.x} ${app.warehouse.y} `;
            route.forEach(elem => {
               d+=`L${elem.x} ${elem.y} `;
            });
            d+=`L${app.warehouse.x} ${app.warehouse.y} `;
            this.canvas.append('path')
                .attr('d', d)
                .attr('stroke', palette[index])
                .attr('stroke-width', 3)
                .attr('fill', 'none')
                .on('mouseover', function() {
                    d3.select(this).attr('stroke-width', 6)
                        .on('mouseout', function() {
                            d3.select(this).attr('stroke-width', 3);
                        });
                });
        });

        return routes;
    };

    clearRoute = () => {
        this.canvas
            .selectAll('path').remove();
    };

    drawPoints = (points) => {
        this.canvas.selectAll('circle')
            .data(points).enter()
            .append('circle')
            .attr('cx', d => d.x)
            .attr('cy', d => d.y)
            .attr('r', '0.4em')
            .on('mouseover', function() {
                d3.select(this).attr('r', '0.8em')
                    .on('mouseout', function() {
                        d3.select(this).attr('r', '0.4em');
                    });
            });
    };

    drawWarehouse = (pos) => {
        this.canvas.selectAll('rect').remove();
        this.canvas.selectAll('rect')
            .data([pos]).enter()
            .append('rect')
            .attr('width', '24px')
            .attr('height', '24px')
            .attr('x', d => d.x - 12)
            .attr('y', d => d.y - 12)
            .attr('fill', '#fff');
    };

    clearPoints = () => {
        this.canvas
            .selectAll('circle')
            .remove();
    }


}
