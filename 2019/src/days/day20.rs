use std::collections::{HashMap, HashSet, VecDeque};
use std::time::Instant;

use aoc_helpers::direction::Direction;
use aoc_helpers::graph::{Edge, Node};
use aoc_helpers::io::parse_char_grid;
use aoc_helpers::point2d::Point2d;
use aoc_helpers::priority_queue::{PriorityQueue, PriorityQueueItem};
use log::{debug, trace};

pub fn solve(file_contents: String) -> (String, String) {
    let parse_timer = Instant::now();
    let input = parse_input(file_contents);
    debug!("File parse: ({:?})", parse_timer.elapsed());

    let part1_timer = Instant::now();
    let part1 = solve_part_1(&input);
    debug!("Part 1: {} ({:?})", part1, part1_timer.elapsed());

    let part2_timer = Instant::now();
    let part2 = solve_part_2(&input);
    debug!("Part 2: {} ({:?})", part2, part2_timer.elapsed());

    (part1.to_string(), part2.to_string())
}

fn parse_input(file_contents: String) -> MazeInfo {
    let grid = parse_char_grid(file_contents);

    let mut portal_entrances: HashMap<String, Vec<Point2d>> = HashMap::new();
    let possible_portal_locations = grid
        .iter()
        .filter(|(_, v)| v.is_ascii_uppercase())
        .collect::<Vec<_>>();

    let directions = [
        (Direction::Right, [Direction::Left, Direction::Right]),
        (Direction::Down, [Direction::Up, Direction::Down]),
    ];

    for (coordinate, &value) in possible_portal_locations {
        trace!("checking if value {} at {} is start of a portal", value, coordinate);
        for (portal_direction, entrance_directions) in &directions {
            let next = coordinate.next(portal_direction);
            trace!("checking if value {} of {} at {} is the end of the portal", portal_direction, coordinate, next);
            if let Some(&c) = grid.get(&next) && c.is_ascii_uppercase() {
                trace!("Found rest of portal {}{} at {}", value, c, next);
                for entrance_direction in entrance_directions {
                    let possible_entrance_locations = [
                        coordinate.next(entrance_direction),
                        next.next(entrance_direction),
                    ];
                    for possible_entrance_location in possible_entrance_locations {
                        trace!("checking if value {} at {} is the entrance to the portal", entrance_direction, possible_entrance_location);
                        if let Some(&e) = grid.get(&possible_entrance_location) && e == '.' {
                            let portal_key = format!("{}{}", value, c);
                            trace!("found entrance to portal {} at {}", portal_key, possible_entrance_location);
                            portal_entrances
                                .entry(portal_key)
                                .and_modify(|p| p.push(possible_entrance_location))
                                .or_insert(vec![possible_entrance_location]);
                            break;
                        }
                    }
                }
                break;
            }
        }
    }

    let graph = convert_to_graph(&grid, &portal_entrances);

    let mut start = Point2d::new(0, 0);
    let mut end = Point2d::new(0, 0);
    let mut portals: HashMap<Point2d, Point2d> = HashMap::new();
    for (name, entrances) in portal_entrances {
        if entrances.len() == 2 {
            trace!("found portal {} between {} <-> {}", name, entrances[0], entrances[1]);
            portals.insert(entrances[0], entrances[1]);
            portals.insert(entrances[1], entrances[0]);
        } else if name == "AA" {
            trace!("found start portal at {}", entrances[0]);
            start = entrances[0];
        } else if name == "ZZ" {
            trace!("found end portal at {}", entrances[0]);
            end = entrances[0];
        } else {
            unreachable!();
        }
    }

    // Find outer edges
    let outer_start_x = 2;
    let outer_start_y = 2;
    let mut point = Point2d::new(outer_start_x, outer_start_y);
    let outer_top_left = point;
    while let Some(&value) = grid.get(&point) {
        if value == ' ' {
            break;
        }
        point.x += 1;
    }
    let outer_end_x = point.x;

    point = Point2d::new(outer_start_x, outer_start_y);
    while let Some(&value) = grid.get(&point) {
        if value == ' ' {
            break;
        }
        point.y += 1;
    }
    let outer_end_y = point.y;

    let outer_bottom_right = Point2d::new(outer_end_x - 1, outer_end_y - 1);

    // Find inner edges
    point = Point2d::new(outer_start_x, outer_start_y);
    while let Some(&value) = grid.get(&point) {
        if value == ' ' {
            break;
        }
        point.x += 1;
        point.y += 1;
    }
    let inner_top_left = Point2d::new(point.x - 1, point.y - 1);

    let maze_width = inner_top_left.x - outer_top_left.x;
    let maze_height = inner_top_left.y - outer_top_left.y;

    let inner_bottom_right = Point2d::new(outer_bottom_right.x - maze_width, outer_bottom_right.y - maze_height);

    let dimensions = Dimensions {
        outer_top_left,
        inner_top_left,
        inner_bottom_right,
        outer_bottom_right,
    };
    trace!("found dimensions: {:?}", dimensions);

    MazeInfo {
        graph,
        portals,
        dimensions,
        start,
        end,
    }
}

fn convert_to_graph(grid: &HashMap<Point2d, char>, portal_entrances: &HashMap<String, Vec<Point2d>>) -> HashMap<Point2d, Node<Point2d>> {
    let mut graph: HashMap<Point2d, Node<Point2d>> = HashMap::new();

    let entrances = portal_entrances
        .iter()
        .flat_map(|(_, entrances)| entrances.iter())
        .collect::<Vec<&Point2d>>();

    for &&entrance in &entrances {
        trace!("searching for portals reachable from {}", entrance);
        let mut reachable_nodes: Vec<Edge<Point2d>> = Vec::new();

        let mut queue: VecDeque<(Point2d, i64)> = VecDeque::new();
        let mut visited: HashSet<Point2d> = HashSet::new();

        queue.push_back((entrance, 0));
        visited.insert(entrance);

        while let Some((coordinate, steps)) = queue.pop_front() {
            if coordinate != entrance && entrances.contains(&&coordinate) {
                trace!("Found portal at {} with distance {} from  {}", coordinate, steps, entrance);
                reachable_nodes.push(Edge::new(coordinate, steps));
            }

            for neighbour in coordinate.neighbours() {
                if !visited.contains(&neighbour) {
                    // in bounds and not seen
                    visited.insert(neighbour);
                    match grid.get(&neighbour) {
                        None => continue,
                        Some('#') => continue,
                        Some('.') => {
                            queue.push_back((neighbour, steps + 1));
                        }
                        _ => continue,
                    }
                }
            }
        }

        graph.insert(entrance, Node::new(reachable_nodes));
    }
    graph
}

fn solve_part_1(input: &MazeInfo) -> i64 {
    trace!("Searching for shortest path between {} and {}", input.start, input.end);

    let mut queue: PriorityQueue<PriorityQueueItem<Point2d>> = PriorityQueue::new();
    let mut visited: HashSet<Point2d> = HashSet::new();

    queue.push(PriorityQueueItem::new(0, input.start));

    while let Some(PriorityQueueItem { weight: steps, data: point }) = queue.pop()
    {
        trace!("Current point: {} ({})", point, steps);

        if point == input.end {
            trace!("found end after {} steps", steps);
            return steps;
        }

        if visited.contains(&point) {
            trace!("Already visited {}", point);
            continue;
        }
        visited.insert(point);

        if let Some(reachable_portals) = input.graph.get(&point) {
            for portal in &reachable_portals.edges {
                trace!("found portal at {} with weight {}", portal.value, portal.weight);

                // enter portal
                if let Some(&portal_exit) = input.portals.get(&portal.value) {
                    trace!("can teleport to {} from {}", portal_exit, portal.value);
                    queue.push(PriorityQueueItem::new(steps + portal.weight + 1, portal_exit));
                } else {
                    trace!("there is no exit to this portal");
                    queue.push(PriorityQueueItem::new(steps + portal.weight, portal.value));
                }
            }
        }
    }
    unreachable!();
}

fn solve_part_2(input: &MazeInfo) -> i64 {
    trace!("Searching for shortest path between {} and {}", input.start, input.end);

    let mut queue: PriorityQueue<PriorityQueueItem<(Point2d, u8)>> = PriorityQueue::new();
    let mut visited: HashSet<(Point2d, u8)> = HashSet::new();

    queue.push(PriorityQueueItem::new(0, (input.start, 0)));
    visited.insert((input.start, 0));

    while let Some(PriorityQueueItem { weight: steps, data: (point, level) }) = queue.pop() {
        trace!("Current point: {} ({})", point, steps);
        if level == 0 && point == input.end {
            trace!("found end after {} steps", steps);
            return steps;
        }

        if let Some(reachable_portals) = input.graph.get(&point) {
            for portal in &reachable_portals.edges {
                trace!("found portal at {} with weight {}", portal.value, portal.weight);
                if visited.contains(&(portal.value, level)) {
                    trace!("Already visited {}", portal.value);
                    continue;
                }
                visited.insert((portal.value, level));

                if let Some(&portal_exit) = input.portals.get(&portal.value) {
                    trace!("can teleport to {} from {}", portal_exit, portal.value);
                    if level > 0 && (portal.value.x == input.dimensions.outer_top_left.x ||
                        portal.value.x == input.dimensions.outer_bottom_right.x ||
                        portal.value.y == input.dimensions.outer_top_left.y ||
                        portal.value.y == input.dimensions.outer_bottom_right.y) {
                        trace!("found outer portal at {} linking to {}. moving from level {} to {}", portal.value, portal_exit, level, level - 1);
                        queue.push(PriorityQueueItem::new(steps + portal.weight + 1, (portal_exit, level - 1)));
                    } else if portal.value.x == input.dimensions.inner_top_left.x ||
                        portal.value.x == input.dimensions.inner_bottom_right.x ||
                        portal.value.y == input.dimensions.inner_top_left.y ||
                        portal.value.y == input.dimensions.inner_bottom_right.y {
                        trace!("found inner portal at {} linking to {}. moving from level {} to {}", portal.value, portal_exit, level, level + 1);
                        queue.push(PriorityQueueItem::new(steps + portal.weight + 1, (portal_exit, level + 1)));
                    }
                } else {
                    trace!("there is no exit to this portal");
                    queue.push(PriorityQueueItem::new(steps + portal.weight, (portal.value, level)));
                }
            }
        }
    }
    unreachable!();
}

#[derive(Debug)]
struct Dimensions {
    outer_top_left: Point2d,
    inner_top_left: Point2d,
    inner_bottom_right: Point2d,
    outer_bottom_right: Point2d,
}

#[derive(Debug)]
struct MazeInfo {
    graph: HashMap<Point2d, Node<Point2d>>,
    portals: HashMap<Point2d, Point2d>,
    dimensions: Dimensions,
    start: Point2d,
    end: Point2d,
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_1() {
        let input: [&str; 2] = [
            "         A
         A
  #######.#########
  #######.........#
  #######.#######.#
  #######.#######.#
  #######.#######.#
  #####  B    ###.#
BC...##  C    ###.#
  ##.##       ###.#
  ##...DE  F  ###.#
  #####    G  ###.#
  #########.#####.#
DE..#######...###.#
  #.#########.###.#
FG..#########.....#
  ###########.#####
             Z
             Z       ",
            "                   A
                   A
  #################.#############
  #.#...#...................#.#.#
  #.#.#.###.###.###.#########.#.#
  #.#.#.......#...#.....#.#.#...#
  #.#########.###.#####.#.#.###.#
  #.............#.#.....#.......#
  ###.###########.###.#####.#.#.#
  #.....#        A   C    #.#.#.#
  #######        S   P    #####.#
  #.#...#                 #......VT
  #.#.#.#                 #.#####
  #...#.#               YN....#.#
  #.###.#                 #####.#
DI....#.#                 #.....#
  #####.#                 #.###.#
ZZ......#               QG....#..AS
  ###.###                 #######
JO..#.#.#                 #.....#
  #.#.#.#                 ###.#.#
  #...#..DI             BU....#..LF
  #####.#                 #.#####
YN......#               VT..#....QG
  #.###.#                 #.###.#
  #.#...#                 #.....#
  ###.###    J L     J    #.#.###
  #.....#    O F     P    #.#...#
  #.###.#####.#.#####.#####.###.#
  #...#.#.#...#.....#.....#.#...#
  #.#####.###.###.#.#.#########.#
  #...#.#.....#...#.#.#.#.....#.#
  #.###.#####.###.###.#.#.#######
  #.#.........#...#.............#
  #########.###.###.#############
           B   J   C
           U   P   P               ",
        ];
        let expected: [i64; 2] = [
            23,
            58,
        ];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_1(&input), expected[i]);
        }
    }

    #[test]
    fn test_part_2() {
        let input: [&str; 2] = [
            "         A
         A
  #######.#########
  #######.........#
  #######.#######.#
  #######.#######.#
  #######.#######.#
  #####  B    ###.#
BC...##  C    ###.#
  ##.##       ###.#
  ##...DE  F  ###.#
  #####    G  ###.#
  #########.#####.#
DE..#######...###.#
  #.#########.###.#
FG..#########.....#
  ###########.#####
             Z
             Z       ",
            "             Z L X W       C
             Z P Q B       K
  ###########.#.#.#.#######.###############
  #...#.......#.#.......#.#.......#.#.#...#
  ###.#.#.#.#.#.#.#.###.#.#.#######.#.#.###
  #.#...#.#.#...#.#.#...#...#...#.#.......#
  #.###.#######.###.###.#.###.###.#.#######
  #...#.......#.#...#...#.............#...#
  #.#########.#######.#.#######.#######.###
  #...#.#    F       R I       Z    #.#.#.#
  #.###.#    D       E C       H    #.#.#.#
  #.#...#                           #...#.#
  #.###.#                           #.###.#
  #.#....OA                       WB..#.#..ZH
  #.###.#                           #.#.#.#
CJ......#                           #.....#
  #######                           #######
  #.#....CK                         #......IC
  #.###.#                           #.###.#
  #.....#                           #...#.#
  ###.###                           #.#.#.#
XF....#.#                         RF..#.#.#
  #####.#                           #######
  #......CJ                       NM..#...#
  ###.#.#                           #.###.#
RE....#.#                           #......RF
  ###.###        X   X       L      #.#.#.#
  #.....#        F   Q       P      #.#.#.#
  ###.###########.###.#######.#########.###
  #.....#...#.....#.......#...#.....#.#...#
  #####.#.###.#######.#######.###.###.#.#.#
  #.......#.......#.#.#.#.#...#...#...#.#.#
  #####.###.#####.#.#.#.#.###.###.#.###.###
  #.......#.....#.#...#...............#...#
  #############.#.#.###.###################
               A O F   N
               A A D   M                     ",
        ];
        let expected: [i64; 2] = [
            26,
            396,
        ];

        for i in 0..input.len() {
            let input = parse_input(input[i].to_string());
            assert_eq!(solve_part_2(&input), expected[i]);
        }
    }
}
