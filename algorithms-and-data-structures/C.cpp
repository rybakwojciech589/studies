#include <bits/stdc++.h>
#include <unistd.h>
using namespace std;

//#define int long long
#define ll long long
#define ld long double
#define pb push_back
#define nd second
#define st first
const ll infl=1e18+90;
const int inf=1e9+93;
const int MAX_N=1e5+13;

vector<pair<int,int> > graph[MAX_N];
int dist[MAX_N];

struct CustomHeap {
    vector<pair<int, int>> elements;

    void sift_up(int idx) {
        while (idx > 0) {
            int parent_idx = (idx - 1) / 2;
            if (elements[idx].first < elements[parent_idx].first) {
                swap(elements[idx], elements[parent_idx]);
                idx = parent_idx;
            } else {
                break;
            }
        }
    }

    void sift_down(int idx) {
        int n = elements.size();
        while (true) {
            int left_child = 2 * idx + 1;
            int right_child = 2 * idx + 2;
            int smallest_idx = idx;

            if (left_child < n && elements[left_child].first < elements[smallest_idx].first) {
                smallest_idx = left_child;
            }
            if (right_child < n && elements[right_child].first < elements[smallest_idx].first) {
                smallest_idx = right_child;
            }

            if (smallest_idx != idx) {
                swap(elements[idx], elements[smallest_idx]);
                idx = smallest_idx;
            } else {
                break;
            }
        }
    }

    void add(pair<int, int> p) {
        elements.push_back(p);
        sift_up(elements.size() - 1);
    }

    pair<int, int> peek() {
        return elements[0];
    }

    void extract_min() {
        if (elements.empty()) return;
        
        elements[0] = elements.back();
        elements.pop_back();
        
        if (!elements.empty()) {
            sift_down(0);
        }
    }

    bool is_empty() {
        return elements.empty();
    }
};

void solve() {
    int n, m, k;
    ll total_dist=0;
    cin >> n >> m >> k;
    for (int i = 0; i < m; i++) {
        int a, b, c;
        cin >> a >> b >> c;
        graph[a].push_back({b, c});
        graph[b].push_back({a, c});
    }
    for (int i = 1; i <= n; i++) {
        dist[i] = inf;
    }
    CustomHeap min_heap;
    min_heap.elements.reserve(m + 7);
    dist[1] = 0;
    min_heap.add({0, 1});
    
    while(!min_heap.is_empty()) {
        pair<int, int> u = min_heap.peek();
        min_heap.extract_min();
        if (u.first > dist[u.second]) continue;
        for (auto v : graph[u.second]) {
            if (dist[u.second] + v.second < dist[v.first]) {
                dist[v.first] = dist[u.second] + v.second;
                min_heap.add({dist[v.first], v.first});
            }
        }
    }
    for(int i=1;i<=k;i++){
        int a; cin>>a;
        if(dist[a]==inf) 
        {
            cout<<"NIE\n";
            return;
        }
        else total_dist+=dist[a]*2;
    }
    cout<<total_dist<<"\n";
}

signed main()
{
    ios_base::sync_with_stdio(false);
    cin.tie(NULL);
    int t=1;
    //cin>>t;
    while(t--)
    {
        solve();
    }
    return 0;
}
