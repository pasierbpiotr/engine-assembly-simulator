// Określa możliwe stany części silnika podczas procesu montażowego
public enum AssemblyState
{
    // Część zablokowana - niedostępna do interakcji
    // (domyślny stan przed rozpoczęciem montażu)
    Locked,
    // Część odblokowana - gotowa do podjęcia przez użytkownika
    // (aktywny stan podczas montażu)
    Unlocked,
    // Część zmontowana - ostateczna pozycja, blokada dalszych interakcji
    // (stan końcowy po prawidłowym montażu)
    Assembled
}
