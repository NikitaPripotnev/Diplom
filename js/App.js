class App {
    constructor() {
        this.requests = {
            pickups: [ ],
                deliveries: [ ]
        };
        this.warehouse = {x: 100, y: 100};
        this.points = [
            {x: 200, y: 211},
            {x: 305, y: 150},
            {x: 326, y: 327},
            {x: 441, y: 333},
            {x: 513, y: 336},
            {x: 638, y: 239},
            {x: 655, y: 207},
            {x: 546, y: 168},
            {x: 439, y: 122},
            {x: 285, y: 42},
            {x: 213, y: 63},
            {x: 183, y: 124},
            {x: 133, y: 220},
            {x: 128, y: 299},
            {x: 215, y: 321},
            {x: 251, y: 305},
            {x: 253, y: 230},
            {x: 262, y: 224},
            {x: 491, y: 522}
        ];
        this.params = {
            fleetSize: 1,
            dRunner: 5,
            dRoot: 5,
            globalIter: 5,
            localIter: 100,
            nPlants: 100,
            neighborhoodSize: 50
        };
        this.pallete=[
            "#990000",
            "#006ac1",
            "#15992a",
            "#f4b300",
            "#e064bc"
        ]
    }
}