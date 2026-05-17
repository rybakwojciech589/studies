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
const int roz=2e6+83;

int d[roz], tab[roz];

void solve() {
    int n, dl=1, maxi=0, wynik=0;
    cin>>n;
    for(int i=0; i<=n; i++) {
        d[i]=inf;
        tab[i]=0;
    }
    d[0]=-inf;
    tab[n]=0;
    for(int i=0; i<=n; i++) {
        if(i!=n) cin>>tab[i];
        if(i==0) {
            continue;
        }
        if(tab[i]>tab[i-1]) {
            dl++;
        }
        else {
            for(int j=i-1;j>=i-dl;j--) {
                int pocz=0, kon=maxi, sr;
                while(pocz<kon-1) {
                    sr=(pocz+kon)/2;
                    if(d[sr]<tab[j]) {
                        pocz=sr;
                    }
                    else {
                        kon=sr;
                    }
                }
                if(d[kon]<tab[j]) {
                    wynik=max(wynik, kon+((i-1)+1-j));
                }
                else {
                    wynik=max(wynik, pocz+((i-1)+1-j));
                }
            }
            for(int j=i-dl;j<i;j++) {
                d[j-(i-dl)+1]=min(d[j-(i-dl)+1], tab[j]);
                maxi=max(maxi, j-(i-dl)+1);
            }
            dl=1;
        }
    }
    cout<<wynik<<"\n";
}

signed main()
{
    ios_base::sync_with_stdio(false);
    cin.tie(NULL);
    int t=1;
    cin>>t;
    while(t--)
    {
        solve();
    }
    return 0;
}
