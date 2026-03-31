# Algorithms and Data Structures 2025/2026

This folder contains my solutions to the algorithmic problems from the 2025/2026 academic year. Full problem descriptions can be found in the attached PDF files.

---

## Task A: Connected Components on a Grid (A.cpp)

**Problem Summary:** The task requires analyzing an `n` x `m` grid filled with characters ('A' to 'F') representing different types of connections (e.g., pipes linking horizontally, vertically, or both). The goal is to calculate the total number of isolated networks (connected components), ignoring empty/disconnected cells. See `2026_A.pdf` for the exact connection rules.

**Solution:**
The problem is solved using a graph traversal approach with heavy memory optimization.
* **Bitwise Graph Representation:** Instead of using a standard adjacency list (which would consume too much memory for a large grid), the graph is compressed using bitwise operations within a `uint8_t graf[roz]` array. Each bit represents a directional connection to a neighboring cell.
* **Iterative BFS:** Although the traversal function is named `dfs`, it implements an iterative Breadth-First Search (BFS) using a `std::queue`. 
* **Component Counting:** The algorithm iterates through the grid. Upon finding an unvisited valid cell, it triggers the BFS to mark the entire connected component and increments the network counter.

---

## Task B: Blocks / Klocki (B.cpp)

**Problem Summary:** Given a set of blocks with specific heights, the goal is to build two towers of exactly the same height. If possible, the program returns the maximum achievable height. If not, it calculates the minimum possible height difference between the two towers. See `2026_B.pdf` for full details.

**Solution:**
This is a variation of the Knapsack / Subset Sum problem, solved using Dynamic Programming (DP). The primary challenge is the strict 16 MB memory limit and the large maximum sum of heights.
* **Memory Optimization (Rolling Arrays):** To stay within the 16 MB limit, a full 2D DP table is avoided. Instead, the solution uses a rolling array technique (`dp[2][roz]`), alternating states between `x` and `!x` to only store the current and previous DP states.
* **Performance Optimization (Frequency Reduction):** The algorithm groups blocks of the same height. To drastically reduce the number of DP transitions, it applies a mathematical trick: if there are 4 or more blocks of height `i`, it merges two of them into a single "virtual" block of height `2 * i`. This preserves all achievable sums while significantly speeding up execution.
* **DP Transitions:** The DP state tracks the maximum total height used across both towers for a specific height difference. For each block, the algorithm evaluates three choices: adding it to the taller tower, adding it to the shorter tower, or skipping it entirely.

---

## Task C: Traveling Salesman / Komiwojażer (C.cpp)

**Problem Summary:** A salesman must deliver items from the capital to a given list of target cities. Since his car only holds one item, he must return to the capital after each delivery. He can visit the target cities in any order, but on his way, he is only allowed to pass through non-target cities or target cities where he has already sold an item. The goal is to find the minimum total distance to complete all deliveries, or output "NIE" if a city is unreachable. Furthermore, there is a strict restriction against using built-in STL priority queues, sets, or maps.

**Solution:**
The problem is solved using Dijkstra's algorithm with custom data structures to comply with the strict limitations.
* **Algorithmic Insight:** The restriction about passing through cities naturally resolves itself if the salesman visits the target cities ordered by their shortest distance from the capital. By doing so, he will never pass through an unvisited target city (since it would have to be closer, contradicting the sorted order). Thus, the total distance is simply the sum of round trips (2 * shortest path) to each target city.
* **Custom Min-Heap:** Because `std::priority_queue` and `std::set` are strictly banned, the solution implements a custom Min-Heap structure (`CustomHeap`) from scratch using a `std::vector` and custom `sift_up` / `sift_down` methods to manage the priority queue operations.
* **Data Types:** The accumulated total distance uses a 64-bit integer (`long long total_dist`) to prevent integer overflow, as the sum of multiple round trips across up to 100,000 cities can easily exceed standard 32-bit limits. The shortest paths are tracked in the `dist` array.
