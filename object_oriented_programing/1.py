def silnia_imp(n):
    wynik = 1
    for i in range(2, n + 1):
        wynik *= i
    return wynik

def silnia_fun(n):
    return 1 if n <= 1 else n * silnia_fun(n - 1)

def binom(n, k):
    if k < 0 or k > n:
        return 0
    return silnia_imp(n) // (silnia_imp(k) * silnia_imp(n - k))

def pascal_row(n):
    return [binom(n, k) for k in range(n + 1)]

def map_imp(lista, func):
    nowa_lista = None
    elementy = []
    temp = lista
    while temp is not None:
        elementy.append(func(temp[0]))
        temp = temp[1]
    for el in reversed(elementy):
        nowa_lista = (el, nowa_lista)
    return nowa_lista

def map_fun(lista, func):
    if lista is None:
        return None
    return (func(lista[0]), map_fun(lista[1], func))

def filter_imp(lista, func):
    elementy = []
    temp = lista
    while temp is not None:
        if func(temp[0]):
            elementy.append(temp[0])
        temp = temp[1]
    nowa_lista = None
    for el in reversed(elementy):
        nowa_lista = (el, nowa_lista)
    return nowa_lista

def filter_fun(lista, func):
    if lista is None:
        return None
    if func(lista[0]):
        return (lista[0], filter_fun(lista[1], func))
    return filter_fun(lista[1], func)

if __name__ == "__main__":
    n_pascal = 10
    print(f"{n_pascal}. wiersz trójkąta Pascala: {pascal_row(n_pascal)}")

    moja_lista = (1, (2, (3, (4, (5, None)))))
    
    kwadraty = map_imp(moja_lista, lambda x: x**2)
    print(f"Lista wejściowa: {moja_lista}")
    print(f"Wynik map_imp:  {kwadraty}")
    kwadraty2 = map_fun(moja_lista, lambda x: x**2)
    print(f"Lista wejściowa: {moja_lista}")
    print(f"Wynik map_fun:  {kwadraty2}")

    parzyste = filter_imp(moja_lista, lambda x: x % 2 == 0)
    print(f"Wynik filter_imp: {parzyste}")
    parzyste2 = filter_fun(moja_lista, lambda x: x % 2 == 0)
    print(f"Wynik filter_fun: {parzyste2}")
