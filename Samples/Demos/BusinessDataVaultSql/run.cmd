cd /d C:\Users\jimmy\Desktop\meta-bi\Samples\Demos\BusinessDataVaultSql
rmdir /s /q GeneratedSql
mkdir GeneratedSql
meta-datavault-business generate-sql --workspace BusinessDataVault --implementation-workspace Implementation --data-type-conversion-workspace DataTypeConversion --out GeneratedSql

