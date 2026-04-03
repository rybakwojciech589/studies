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
const int roz=1e6+13;

int tab[roz], dp[2][roz], current_sum=0;

void solve() {
    int n;
    cin>>n;
    for(int i=1;i<=n;i++)
    {
        int a; cin>>a;
        tab[a]++;
    }
    bool x=0;
    for(int i=1;i<=roz-9;i++)
    {
        while(tab[i]>=4)
        {
            tab[i]-=2;
            tab[i*2]++;
        }
        for(int j=1;j<=tab[i];j++)
        {
            for(int k=0;k<=current_sum;k++)
            {
                dp[!x][k]=0;   
            }
            for(int k=0;k<=current_sum;k++)
            {
                if(k!=0&&dp[x][k]==0) continue;
                dp[!x][k+i]=max(dp[!x][k+i], dp[x][k]+i);
                dp[!x][abs(k-i)]=max(dp[!x][abs(k-i)], dp[x][k]+i);   
                dp[!x][k]=max(dp[!x][k], dp[x][k]);
            }
            current_sum+=i;
            x=!x;
        }
    }
    if(dp[x][0]>0)
    {
        cout<<"TAK\n";
        cout<<dp[x][0]/2<<"\n";
    }
    else 
    {
        cout<<"NIE\n";
        for(int i=1;i<=roz-2;i++)
        {
            if(dp[x][i]>0&&dp[x][i]!=i)
            {
                cout<<i<<"\n";
                break;
            }
        }
    }
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
