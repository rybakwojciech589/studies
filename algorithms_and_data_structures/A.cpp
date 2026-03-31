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
const int roz=4e6+13;

bitset<roz> odw;
uint8_t graf[roz];
int n, m;
queue<int> kol;

void dfs(int x)
{
    odw[x]=1;
    kol.push(x);
    while(!kol.empty())
    {
        int v=kol.front();
        kol.pop();
        for(int i=0;i<4;i++)
        {
            if((graf[v]&(1<<i))!=0)
            {
                int u=v;
                if(i==0) u++;
                else if(i==1) u+=m;
                else if(i==2) u-=m;
                else u--;
                if(odw[u]==0)
                {
                    odw[u]=1;
                    kol.push(u);
                }
            }
        }
    }
}

void solve() {
    cin>>n>>m;
    string s="";
    string a="";
    s.reserve(n * m + 7);
    for(int i=0;i<n;i++)
    {
        cin>>a;
        s+=a;
    }
    for(int i=0;i<n;i++)
    {
        for(int j=0;j<m;j++)
        {
            if(s[i*m+j]=='B')
            {
                if(i!=n-1 && (s[i*m+j+m]=='C' || s[i*m+j+m]=='D' || s[i*m+j+m]=='F'))
                {
                    graf[i*m+j]+=2;
                    graf[i*m+j+m]+=4;
                }
            }
            else if(s[i*m+j]=='D')
            {
                if(j!=m-1 && (s[i*m+j+1]=='C' || s[i*m+j+1]=='B' || s[i*m+j+1]=='F'))
                {
                    graf[i*m+j]+=1;
                    graf[i*m+j+1]+=8;
                }
            }
             else if(s[i*m+j]=='E'||s[i*m+j]=='F')
            {
                if(i!=n-1 && (s[i*m+j+m]=='C' || s[i*m+j+m]=='D' || s[i*m+j+m]=='F'))
                {
                    graf[i*m+j]+=2;
                    graf[i*m+j+m]+=4;
                }
                if(j!=m-1 && (s[i*m+j+1]=='C' || s[i*m+j+1]=='B' || s[i*m+j+1]=='F'))
                {
                    graf[i*m+j]+=1;
                    graf[i*m+j+1]+=8;
                }
            }
        }
    }
    int licz=0;
    for(int i=0;i<n*m;i++)
    {
        if(s[i]!='A')
        {
            if(odw[i]==0)
            {
                dfs(i);
                licz++;
            }
        }
    }
    cout<<licz<<"\n";
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
