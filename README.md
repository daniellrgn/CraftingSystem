readme

In the crafting scene, Database Name must be defined within ItemDB, RecipeDB, and Inventory.

New tables can be created in the database by going through the DatabaseTableTool and entering values 
into the table attributes list as if entering SQL commands.

Table deletion not implemented in order to prevent accidental table drops.

Pressing "v" while pointed at the chest in the craft scene will open an inventory with one of every item
within the item table

Within the loot inventory game object, setting the number of slots will change the number of slots that appear
within the loot inventory panel, but the size of the panel won't adjust to the number of slots.

Min distance within the chest script determines the minimum distance a player must be to open the chest. 