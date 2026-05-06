module Zapisywalny
  def zapisz_do_pliku(nazwa_pliku)
    File.open(nazwa_pliku, "wb") { |plik| Marshal.dump(self, plik) }
  end

  module ClassMethods
    def wczytaj_z_pliku(nazwa_pliku)
      File.open(nazwa_pliku, "rb") { |plik| Marshal.load(plik) }
    end
  end

  def self.included(base)
    base.extend(ClassMethods)
  end
end


class GraDwuosobowa
  def graj
    catch(:zapisz_gre) do
      maksymalna_liczba_tur.times do
        break if koniec?
        tura
      end

      wyswietl_wynik
      return
    end

    obsluz_zapis
  end

  private

  def maksymalna_liczba_tur
    1000
  end

  def koniec?
    raise NotImplementedError
  end

  def tura
    raise NotImplementedError
  end

  def wyswietl_wynik
    raise NotImplementedError
  end

  def obsluz_zapis
    raise NotImplementedError
  end
end

 
class Karta
  include Comparable
  attr_reader :figura, :kolor, :wartosc

  @@karty = {}
  FIGURY = {'2'=>2, '3'=>3, '4'=>4, '5'=>5, '6'=>6, '7'=>7, '8'=>8, '9'=>9, '10'=>10, 'J'=>11, 'D'=>12, 'K'=>13, 'A'=>14}
  KOLORY = ['kier', 'karo', 'trefl', 'pik']

  def initialize(f, k) @figura, @kolor, @wartosc = f, k, FIGURY[f] end

  def self.pobierz(f, k) @@karty["#{f}#{k}"] ||= new(f, k) end

  def self.talia
    FIGURY.keys.product(KOLORY).map { |f, k| pobierz(f, k) }.shuffle
  end

  def <=>(inna) @wartosc <=> inna.wartosc end
  def to_s() "#{@figura} #{@kolor}" end
end

class GraczCzlowiek
  attr_accessor :karty, :nick
  def initialize(nick) @nick, @karty = nick, [] end
  def wez(k) @karty += k end
  def ma_karty?() @karty.any? end
  def rzuc
    print "[#{@nick}] Wciśnij Enter by zagrać, wpisz 'z' aby zapisać i wyjść (karty: #{@karty.size})... "
    wejscie = gets.chomp.downcase
    throw :zapisz_gre if wejscie == 'z'
    @karty.shift
  end
end

class GraczKomputer
  attr_accessor :karty, :nick
  def initialize(nick) @nick, @karty = nick, [] end
  def wez(k) @karty += k end
  def ma_karty?() @karty.any? end
  def rzuc() @karty.shift end
end


class Gra < GraDwuosobowa
  include Zapisywalny

  def initialize(g1, g2)
    @g1, @g2, @stol = g1, g2, []
    t = Karta.talia
    @g1.wez(t[0..25]) 
    @g2.wez(t[26..51]) 
  end

  private

  def koniec?
    !(@g1.ma_karty? && @g2.ma_karty?)
  end

  def tura
    k1 = @g1.rzuc
    k2 = @g2.rzuc

    return unless k1 && k2

    @stol.push(k1, k2)

    puts "#{@g1.nick}: #{k1}  VS  #{@g2.nick}: #{k2}"

    if k1 > k2
      przyznaj_karty(@g1)
    elsif k2 > k1
      przyznaj_karty(@g2)
    else
      puts " --- WOJNA --- "
      @stol.push(@g1.rzuc, @g2.rzuc).compact!
      tura
    end
  end

  def przyznaj_karty(gracz)
    puts "=> #{gracz.nick} zgarnia #{@stol.size} kart!"

    gracz.wez(@stol.shuffle)
    @stol.clear

    puts "   Stan kart -> #{@g1.nick}: #{@g1.karty.size} | #{@g2.nick}: #{@g2.karty.size}\n\n"
  end

  def wyswietl_wynik
    wynik =
      if @g1.ma_karty? && !@g2.ma_karty?
        @g1.nick
      elsif @g2.ma_karty? && !@g1.ma_karty?
        @g2.nick
      else
        "Nikt (Remis - przerwano po 1000 tur)"
      end

    puts "\n*** Wygrywa: #{wynik} ***"
  end

  def obsluz_zapis
    zapisz_do_pliku("stan_gry.dat")

    puts "\n--- STAN GRY ZAPISANY DO PLIKU 'stan_gry.dat' ---"
    puts "Do zobaczenia następnym razem!"
  end
end


puts "Czy chcesz wczytać zapisaną grę z pliku 'stan_gry.dat'? (t/n)"

odpowiedz = gets.chomp.downcase

gra = nil

if odpowiedz == 't' && File.exist?("stan_gry.dat")
  gra = Gra.wczytaj_z_pliku("stan_gry.dat")

  puts "\n*** Wznowiono zapisaną grę ***\n\n"
else
  gra = Gra.new(GraczCzlowiek.new("Człowiek"), GraczKomputer.new("Bot"))

  puts "\n*** Rozpoczęto nową grę ***\n\n"
end

gra.graj
