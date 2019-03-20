var app = new App();
const canvas = document.querySelector('svg');
//TODO заменить фиксироыванную высоту
const offsetY = 40;
const drawer = new Drawer();
const solver = new Solver();
console.log("start");

function ready(){
    console.log("ready", canvas);
    canvas.addEventListener('click', function(e){
        if(e.ctrlKey){
            app.warehouse = {x:e.clientX, y:e.clientY-offsetY};
            drawer.clearRoute();
            drawer.drawWarehouse(app.warehouse);
        }
        else{
        app.points.push({x:e.clientX, y:e.clientY-offsetY});
        drawer.drawPoints(app.points);
        }
    } );

    document.querySelector('#calculate-button').addEventListener('click', ()=>{
        console.log(app.points, "points");
        drawer.clearRoute();
        drawer.drawRoute(solver.findRoute(app.points, app.fleetSize).solution, app.pallete);
    });

    document.querySelector('#clear-button').addEventListener('click', ()=>{
        app.points=[];
        drawer.clearPoints();
        drawer.clearRoute();
    });


    drawer.drawWarehouse(app.warehouse);
    drawer.drawPoints(app.points);
}
document.addEventListener('DOMContentLoaded', ready);
