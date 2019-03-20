class Solver{
    constructor(){};
    globalSearch = (population) => {
        //console.log(population, "population in GLOBALSEARCH");
        return population.map(sol => this.bestSolution(this.globalNeighborhood(sol)).solution);
    };

    globalNeighborhood = (solution) => {
        const improve = function(route, index) {
            const clone = _.clone(solution, true);
            return _(route).shuffle().thru(value => {
                const toRemove = Math.floor(Math.random() * app.params.dRunner);
                clone[index] = _.drop(value, toRemove);
                return _.take(value, toRemove);
            }).map(function(point) {
                _.sample(clone).push(point);
                return clone;
            }).value();
        };

        return solution.map(improve).reduce((x, y) => x.concat(y), []);
    };

    pick = (list, size) => {
        const randomIndex = list => Math.floor(Math.random() * list.length);
        const removed = [];
        _.each(Array(size), () => {
            removed.push(list.splice(randomIndex(list), 1).pop());
        });

        return removed;
    };

    localNeighborhood = (solution) => {
        const switchRoute = (route) => {
            const clone = _.clone(route, true),
                toRemove = Math.min(clone.length,
                    Math.floor(app.params.dRoot * Math.random())),
                removed = this.pick(clone, toRemove);
            return clone.concat(removed);
        };
        const improve = solution => solution.map(switchRoute);

        return _.map(Array(app.params.neighborhoodSize), _ => improve(solution));
    };

    localSearch = (solution) => {
        return this.bestSolution(this.localNeighborhood(solution));
    };

    iterate = (population, cost) => {
        let iter = 0,
            bestCost = cost,
            bestPopulation = population,
        currentBest, current, currentCost;
        //console.log(population, cost, "population and cost in ITERATE");

        while (iter < app.params.localIter) {
            current = this.globalSearch(population);
            currentBest = this.bestSolution(current);
            currentCost = currentBest.cost;

            if (currentCost < bestCost) {
                bestCost = currentCost;
                population = current;
                iter = 0;
            }
            else {
                ++iter;
            }
        }

        iter = 0;
        let best = currentBest;
        while (iter < app.params.localIter) {
            const currentSolution = this.localSearch(best.solution);
            if (currentSolution.cost < best.cost) {
                bestCost = currentSolution.cost;
                best = currentSolution;
                iter = 0;
            }
            else {
                ++iter;
            }
        }
        return best;
    };

    solutionCost = (solution) => {
        // change this for non-euclidean cost functions
        const distanceFn = (p1, p2) => {
            if (!(p1 && p2)) { return 0; }
            const square = x => x * x;
            return Math.sqrt(square(p2.x - p1.x) + square(p2.y - p1.y));
        };
        const routeCost = function(route) {
            if (!route) return 0;
            let acc = distanceFn(app.warehouse, route[0]);

            route.slice(1).forEach(function(_, index) {
                acc += distanceFn(route[index - 1], route[index]);
            });

            return acc + distanceFn(_.last(route), app.warehouse);
        };

        return solution.map(route => routeCost(route))
            .reduce((a, b) => a + b, 0);
    };

    bestSolution = (population) => {
        return population.reduce((prev, cur) => {
            // Sometimes we might get empty solutions!
            //console.log(population, prev, cur, "cur in BESTSOLUTION");
            if (!cur.length) {
                return prev;
            }

            const cost = this.solutionCost(cur);
            //console.log(cost, "cost in BESTSOLUTION");
            if (this.solutionCost(cur) < prev.cost) {
                return { solution: cur, cost: cost };
            }
            return prev;
        }, { solution: [], cost: Infinity });
    };

    initialSolution = (points) => {
        const chunked = _.chunk(points, Math.ceil(points.length/app.params.fleetSize));
        return chunked.concat(_.fill(Array(app.params.fleetSize - chunked.length), []));
    };

    initialPopulation = (points) => {
        //console.log();
        return _.map(Array(app.params.nPlants), _ => this.initialSolution(points));
    };

    findRoute = (points, fleetSize) => {
        const initial = this.initialPopulation(points);
        return this.iterate(initial, this.bestSolution(initial).cost);
    }
}