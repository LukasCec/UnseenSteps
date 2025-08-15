-> merchant

=== merchant ===
You stop at a small stall where a merchant greets you. "Hello, traveler! What do you want?"
    + [View goods]
        #OPEN_SHOP:ELIXIRS
        -> examine
    + [Negotiate price]
        -> negotiate
    + [Leave]
        -> leave

=== examine ===
You quickly scan his items. They seem modest but interesting.
-> END

=== negotiate ===
You ask for a better deal. The merchant smiles and offers a discount.
-> END

=== leave ===
You thank him and walk away.
-> END
