// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaTabular;
public sealed class MetaTabularWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaTabular.MetaTabularModel>
{
    private const string ContractSignature = "6d6f64656c7c4d657461546162756c61720a656e746974797c546162756c617243616c63756c6174696f6e47726f75707c546162756c617243616c63756c6174696f6e47726f75704c6973740a70726f70657274797c546162756c617243616c63756c6174696f6e47726f75707c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c617243616c63756c6174696f6e47726f75707c4e616d657c72657175697265640a70726f70657274797c546162756c617243616c63756c6174696f6e47726f75707c507265636564656e63657c72657175697265640a72656c6174696f6e736869707c546162756c617243616c63756c6174696f6e47726f75707c546162756c61724d6f64656c7c546162756c61724d6f64656c49647c72657175697265640a656e746974797c546162756c617243616c63756c6174696f6e4974656d7c546162756c617243616c63756c6174696f6e4974656d4c6973740a70726f70657274797c546162756c617243616c63756c6174696f6e4974656d7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c617243616c63756c6174696f6e4974656d7c45787072657373696f6e7c72657175697265640a70726f70657274797c546162756c617243616c63756c6174696f6e4974656d7c466f726d6174537472696e6745787072657373696f6e7c6e756c6c61626c650a70726f70657274797c546162756c617243616c63756c6174696f6e4974656d7c4e616d657c72657175697265640a70726f70657274797c546162756c617243616c63756c6174696f6e4974656d7c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c546162756c617243616c63756c6174696f6e4974656d7c546162756c617243616c63756c6174696f6e47726f75707c546162756c617243616c63756c6174696f6e47726f757049647c72657175697265640a656e746974797c546162756c6172436f6c756d6e7c546162756c6172436f6c756d6e4c6973740a70726f70657274797c546162756c6172436f6c756d6e7c4461746143617465676f72797c6e756c6c61626c650a70726f70657274797c546162756c6172436f6c756d6e7c446174615479706549647c72657175697265640a70726f70657274797c546162756c6172436f6c756d6e7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c6172436f6c756d6e7c45787072657373696f6e7c6e756c6c61626c650a70726f70657274797c546162756c6172436f6c756d6e7c466f726d6174537472696e677c6e756c6c61626c650a70726f70657274797c546162756c6172436f6c756d6e7c497348696464656e7c6e756c6c61626c650a70726f70657274797c546162756c6172436f6c756d6e7c49734b65797c6e756c6c61626c650a70726f70657274797c546162756c6172436f6c756d6e7c49734e756c6c61626c657c6e756c6c61626c650a70726f70657274797c546162756c6172436f6c756d6e7c4e616d657c72657175697265640a70726f70657274797c546162756c6172436f6c756d6e7c4f7264696e616c7c72657175697265640a70726f70657274797c546162756c6172436f6c756d6e7c536f757263654e616d657c6e756c6c61626c650a70726f70657274797c546162756c6172436f6c756d6e7c53756d6d6172697a6542797c6e756c6c61626c650a72656c6174696f6e736869707c546162756c6172436f6c756d6e7c546162756c61725461626c657c546162756c61725461626c6549647c72657175697265640a656e746974797c546162756c6172436f6c756d6e5065726d697373696f6e7c546162756c6172436f6c756d6e5065726d697373696f6e4c6973740a70726f70657274797c546162756c6172436f6c756d6e5065726d697373696f6e7c4d657461646174615065726d697373696f6e7c72657175697265640a72656c6174696f6e736869707c546162756c6172436f6c756d6e5065726d697373696f6e7c546162756c6172436f6c756d6e7c546162756c6172436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c546162756c6172436f6c756d6e5065726d697373696f6e7c546162756c61725365637572697479526f6c657c546162756c61725365637572697479526f6c6549647c72657175697265640a656e746974797c546162756c6172436f6c756d6e5472616e736c6174696f6e7c546162756c6172436f6c756d6e5472616e736c6174696f6e4c6973740a70726f70657274797c546162756c6172436f6c756d6e5472616e736c6174696f6e7c43617074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c6172436f6c756d6e5472616e736c6174696f6e7c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c546162756c6172436f6c756d6e5472616e736c6174696f6e7c546162756c6172436f6c756d6e7c546162756c6172436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c546162756c6172436f6c756d6e5472616e736c6174696f6e7c546162756c617243756c747572657c546162756c617243756c7475726549647c72657175697265640a656e746974797c546162756c617243756c747572657c546162756c617243756c747572654c6973740a70726f70657274797c546162756c617243756c747572657c4e616d657c72657175697265640a72656c6174696f6e736869707c546162756c617243756c747572657c546162756c61724d6f64656c7c546162756c61724d6f64656c49647c72657175697265640a656e746974797c546162756c617244617461536f757263657c546162756c617244617461536f757263654c6973740a70726f70657274797c546162756c617244617461536f757263657c436f6e6e656374696f6e5265666572656e63657c6e756c6c61626c650a70726f70657274797c546162756c617244617461536f757263657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c617244617461536f757263657c4e616d657c72657175697265640a70726f70657274797c546162756c617244617461536f757263657c50726f76696465727c6e756c6c61626c650a72656c6174696f6e736869707c546162756c617244617461536f757263657c546162756c61724d6f64656c7c546162756c61724d6f64656c49647c72657175697265640a656e746974797c546162756c61724869657261726368797c546162756c61724869657261726368794c6973740a70726f70657274797c546162756c61724869657261726368797c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c61724869657261726368797c446973706c6179466f6c6465727c6e756c6c61626c650a70726f70657274797c546162756c61724869657261726368797c497348696464656e7c6e756c6c61626c650a70726f70657274797c546162756c61724869657261726368797c4e616d657c72657175697265640a72656c6174696f6e736869707c546162756c61724869657261726368797c546162756c61725461626c657c546162756c61725461626c6549647c72657175697265640a656e746974797c546162756c61724869657261726368794c6576656c7c546162756c61724869657261726368794c6576656c4c6973740a70726f70657274797c546162756c61724869657261726368794c6576656c7c4e616d657c72657175697265640a70726f70657274797c546162756c61724869657261726368794c6576656c7c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c546162756c61724869657261726368794c6576656c7c546162756c6172436f6c756d6e7c546162756c6172436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c546162756c61724869657261726368794c6576656c7c546162756c61724869657261726368797c546162756c617248696572617263687949647c72657175697265640a656e746974797c546162756c61724869657261726368795472616e736c6174696f6e7c546162756c61724869657261726368795472616e736c6174696f6e4c6973740a70726f70657274797c546162756c61724869657261726368795472616e736c6174696f6e7c43617074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c61724869657261726368795472616e736c6174696f6e7c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c546162756c61724869657261726368795472616e736c6174696f6e7c546162756c617243756c747572657c546162756c617243756c7475726549647c72657175697265640a72656c6174696f6e736869707c546162756c61724869657261726368795472616e736c6174696f6e7c546162756c61724869657261726368797c546162756c617248696572617263687949647c72657175697265640a656e746974797c546162756c61724b70697c546162756c61724b70694c6973740a70726f70657274797c546162756c61724b70697c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c61724b70697c53746174757345787072657373696f6e7c6e756c6c61626c650a70726f70657274797c546162756c61724b70697c537461747573477261706869637c6e756c6c61626c650a70726f70657274797c546162756c61724b70697c54617267657445787072657373696f6e7c6e756c6c61626c650a70726f70657274797c546162756c61724b70697c5472656e6445787072657373696f6e7c6e756c6c61626c650a70726f70657274797c546162756c61724b70697c5472656e64477261706869637c6e756c6c61626c650a72656c6174696f6e736869707c546162756c61724b70697c546162756c61724d6561737572657c426173654d65617375726549647c72657175697265640a72656c6174696f6e736869707c546162756c61724b70697c546162756c61724d6561737572657c5461726765744d65617375726549647c6e756c6c61626c650a656e746974797c546162756c61724b70695472616e736c6174696f6e7c546162756c61724b70695472616e736c6174696f6e4c6973740a70726f70657274797c546162756c61724b70695472616e736c6174696f6e7c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c546162756c61724b70695472616e736c6174696f6e7c546162756c617243756c747572657c546162756c617243756c7475726549647c72657175697265640a72656c6174696f6e736869707c546162756c61724b70695472616e736c6174696f6e7c546162756c61724b70697c546162756c61724b706949647c72657175697265640a656e746974797c546162756c61724d6561737572657c546162756c61724d6561737572654c6973740a70726f70657274797c546162756c61724d6561737572657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c61724d6561737572657c446973706c6179466f6c6465727c6e756c6c61626c650a70726f70657274797c546162756c61724d6561737572657c45787072657373696f6e7c6e756c6c61626c650a70726f70657274797c546162756c61724d6561737572657c466f726d6174537472696e677c6e756c6c61626c650a70726f70657274797c546162756c61724d6561737572657c497348696464656e7c6e756c6c61626c650a70726f70657274797c546162756c61724d6561737572657c4e616d657c72657175697265640a72656c6174696f6e736869707c546162756c61724d6561737572657c546162756c61725461626c657c546162756c61725461626c6549647c72657175697265640a656e746974797c546162756c61724d6561737572655472616e736c6174696f6e7c546162756c61724d6561737572655472616e736c6174696f6e4c6973740a70726f70657274797c546162756c61724d6561737572655472616e736c6174696f6e7c43617074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c61724d6561737572655472616e736c6174696f6e7c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c546162756c61724d6561737572655472616e736c6174696f6e7c546162756c617243756c747572657c546162756c617243756c7475726549647c72657175697265640a72656c6174696f6e736869707c546162756c61724d6561737572655472616e736c6174696f6e7c546162756c61724d6561737572657c546162756c61724d65617375726549647c72657175697265640a656e746974797c546162756c61724d6f64656c7c546162756c61724d6f64656c4c6973740a70726f70657274797c546162756c61724d6f64656c7c436f6c6c6174696f6e7c6e756c6c61626c650a70726f70657274797c546162756c61724d6f64656c7c436f6d7061746962696c6974794c6576656c7c6e756c6c61626c650a70726f70657274797c546162756c61724d6f64656c7c44656661756c7443756c747572657c6e756c6c61626c650a70726f70657274797c546162756c61724d6f64656c7c44656661756c7444617461566965777c6e756c6c61626c650a70726f70657274797c546162756c61724d6f64656c7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c61724d6f64656c7c4e616d657c72657175697265640a656e746974797c546162756c6172506172746974696f6e7c546162756c6172506172746974696f6e4c6973740a70726f70657274797c546162756c6172506172746974696f6e7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c6172506172746974696f6e7c45787072657373696f6e7c6e756c6c61626c650a70726f70657274797c546162756c6172506172746974696f6e7c4d6f64657c6e756c6c61626c650a70726f70657274797c546162756c6172506172746974696f6e7c4e616d657c72657175697265640a70726f70657274797c546162756c6172506172746974696f6e7c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c546162756c6172506172746974696f6e7c546162756c617244617461536f757263657c546162756c617244617461536f7572636549647c6e756c6c61626c650a72656c6174696f6e736869707c546162756c6172506172746974696f6e7c546162756c61725461626c657c546162756c61725461626c6549647c72657175697265640a656e746974797c546162756c617250657273706563746976657c546162756c617250657273706563746976654c6973740a70726f70657274797c546162756c617250657273706563746976657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c617250657273706563746976657c4e616d657c72657175697265640a72656c6174696f6e736869707c546162756c617250657273706563746976657c546162756c61724d6f64656c7c546162756c61724d6f64656c49647c72657175697265640a656e746974797c546162756c6172506572737065637469766543616c63756c6174696f6e47726f75707c546162756c6172506572737065637469766543616c63756c6174696f6e47726f75704c6973740a72656c6174696f6e736869707c546162756c6172506572737065637469766543616c63756c6174696f6e47726f75707c546162756c617243616c63756c6174696f6e47726f75707c546162756c617243616c63756c6174696f6e47726f757049647c72657175697265640a72656c6174696f6e736869707c546162756c6172506572737065637469766543616c63756c6174696f6e47726f75707c546162756c617250657273706563746976657c546162756c6172506572737065637469766549647c72657175697265640a656e746974797c546162756c61725065727370656374697665436f6c756d6e7c546162756c61725065727370656374697665436f6c756d6e4c6973740a72656c6174696f6e736869707c546162756c61725065727370656374697665436f6c756d6e7c546162756c6172436f6c756d6e7c546162756c6172436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c546162756c61725065727370656374697665436f6c756d6e7c546162756c617250657273706563746976657c546162756c6172506572737065637469766549647c72657175697265640a656e746974797c546162756c617250657273706563746976654869657261726368797c546162756c617250657273706563746976654869657261726368794c6973740a72656c6174696f6e736869707c546162756c617250657273706563746976654869657261726368797c546162756c61724869657261726368797c546162756c617248696572617263687949647c72657175697265640a72656c6174696f6e736869707c546162756c617250657273706563746976654869657261726368797c546162756c617250657273706563746976657c546162756c6172506572737065637469766549647c72657175697265640a656e746974797c546162756c617250657273706563746976654b70697c546162756c617250657273706563746976654b70694c6973740a72656c6174696f6e736869707c546162756c617250657273706563746976654b70697c546162756c61724b70697c546162756c61724b706949647c72657175697265640a72656c6174696f6e736869707c546162756c617250657273706563746976654b70697c546162756c617250657273706563746976657c546162756c6172506572737065637469766549647c72657175697265640a656e746974797c546162756c617250657273706563746976654d6561737572657c546162756c617250657273706563746976654d6561737572654c6973740a72656c6174696f6e736869707c546162756c617250657273706563746976654d6561737572657c546162756c61724d6561737572657c546162756c61724d65617375726549647c72657175697265640a72656c6174696f6e736869707c546162756c617250657273706563746976654d6561737572657c546162756c617250657273706563746976657c546162756c6172506572737065637469766549647c72657175697265640a656e746974797c546162756c617250657273706563746976655461626c657c546162756c617250657273706563746976655461626c654c6973740a72656c6174696f6e736869707c546162756c617250657273706563746976655461626c657c546162756c617250657273706563746976657c546162756c6172506572737065637469766549647c72657175697265640a72656c6174696f6e736869707c546162756c617250657273706563746976655461626c657c546162756c61725461626c657c546162756c61725461626c6549647c72657175697265640a656e746974797c546162756c617250657273706563746976655472616e736c6174696f6e7c546162756c617250657273706563746976655472616e736c6174696f6e4c6973740a70726f70657274797c546162756c617250657273706563746976655472616e736c6174696f6e7c43617074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c617250657273706563746976655472616e736c6174696f6e7c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c546162756c617250657273706563746976655472616e736c6174696f6e7c546162756c617243756c747572657c546162756c617243756c7475726549647c72657175697265640a72656c6174696f6e736869707c546162756c617250657273706563746976655472616e736c6174696f6e7c546162756c617250657273706563746976657c546162756c6172506572737065637469766549647c72657175697265640a656e746974797c546162756c617252656c6174696f6e736869707c546162756c617252656c6174696f6e736869704c6973740a70726f70657274797c546162756c617252656c6174696f6e736869707c43617264696e616c6974797c72657175697265640a70726f70657274797c546162756c617252656c6174696f6e736869707c43726f737346696c746572446972656374696f6e7c6e756c6c61626c650a70726f70657274797c546162756c617252656c6174696f6e736869707c49734163746976657c6e756c6c61626c650a70726f70657274797c546162756c617252656c6174696f6e736869707c497352657175697265647c6e756c6c61626c650a70726f70657274797c546162756c617252656c6174696f6e736869707c4e616d657c72657175697265640a72656c6174696f6e736869707c546162756c617252656c6174696f6e736869707c546162756c6172436f6c756d6e7c46726f6d436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c546162756c617252656c6174696f6e736869707c546162756c61725461626c657c46726f6d5461626c6549647c72657175697265640a72656c6174696f6e736869707c546162756c617252656c6174696f6e736869707c546162756c6172436f6c756d6e7c546f436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c546162756c617252656c6174696f6e736869707c546162756c61725461626c657c546f5461626c6549647c72657175697265640a656e746974797c546162756c6172526f6c6546696c7465727c546162756c6172526f6c6546696c7465724c6973740a70726f70657274797c546162756c6172526f6c6546696c7465727c45787072657373696f6e7c72657175697265640a72656c6174696f6e736869707c546162756c6172526f6c6546696c7465727c546162756c61725365637572697479526f6c657c546162756c61725365637572697479526f6c6549647c72657175697265640a72656c6174696f6e736869707c546162756c6172526f6c6546696c7465727c546162756c61725461626c657c546162756c61725461626c6549647c72657175697265640a656e746974797c546162756c6172526f6c654d656d6265727c546162756c6172526f6c654d656d6265724c6973740a70726f70657274797c546162756c6172526f6c654d656d6265727c4d656d62657249647c6e756c6c61626c650a70726f70657274797c546162756c6172526f6c654d656d6265727c4d656d6265724e616d657c72657175697265640a72656c6174696f6e736869707c546162756c6172526f6c654d656d6265727c546162756c61725365637572697479526f6c657c546162756c61725365637572697479526f6c6549647c72657175697265640a656e746974797c546162756c61725365637572697479526f6c657c546162756c61725365637572697479526f6c654c6973740a70726f70657274797c546162756c61725365637572697479526f6c657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c61725365637572697479526f6c657c4e616d657c72657175697265640a70726f70657274797c546162756c61725365637572697479526f6c657c5065726d697373696f6e7c72657175697265640a72656c6174696f6e736869707c546162756c61725365637572697479526f6c657c546162756c61724d6f64656c7c546162756c61724d6f64656c49647c72657175697265640a656e746974797c546162756c6172536f72744279436f6c756d6e7c546162756c6172536f72744279436f6c756d6e4c6973740a72656c6174696f6e736869707c546162756c6172536f72744279436f6c756d6e7c546162756c6172436f6c756d6e7c536f7274436f6c756d6e49647c72657175697265640a72656c6174696f6e736869707c546162756c6172536f72744279436f6c756d6e7c546162756c6172436f6c756d6e7c536f75726365436f6c756d6e49647c72657175697265640a656e746974797c546162756c61725461626c657c546162756c61725461626c654c6973740a70726f70657274797c546162756c61725461626c657c4461746143617465676f72797c6e756c6c61626c650a70726f70657274797c546162756c61725461626c657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c61725461626c657c497348696464656e7c6e756c6c61626c650a70726f70657274797c546162756c61725461626c657c4e616d657c72657175697265640a72656c6174696f6e736869707c546162756c61725461626c657c546162756c61724d6f64656c7c546162756c61724d6f64656c49647c72657175697265640a656e746974797c546162756c61725461626c655065726d697373696f6e7c546162756c61725461626c655065726d697373696f6e4c6973740a70726f70657274797c546162756c61725461626c655065726d697373696f6e7c4d657461646174615065726d697373696f6e7c72657175697265640a72656c6174696f6e736869707c546162756c61725461626c655065726d697373696f6e7c546162756c61725365637572697479526f6c657c546162756c61725365637572697479526f6c6549647c72657175697265640a72656c6174696f6e736869707c546162756c61725461626c655065726d697373696f6e7c546162756c61725461626c657c546162756c61725461626c6549647c72657175697265640a656e746974797c546162756c61725461626c655472616e736c6174696f6e7c546162756c61725461626c655472616e736c6174696f6e4c6973740a70726f70657274797c546162756c61725461626c655472616e736c6174696f6e7c43617074696f6e7c6e756c6c61626c650a70726f70657274797c546162756c61725461626c655472616e736c6174696f6e7c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c546162756c61725461626c655472616e736c6174696f6e7c546162756c617243756c747572657c546162756c617243756c7475726549647c72657175697265640a72656c6174696f6e736869707c546162756c61725461626c655472616e736c6174696f6e7c546162756c61725461626c657c546162756c61725461626c6549647c7265717569726564";
    public global::MetaTabular.MetaTabularModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaTabular.MetaTabularModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularCalculationGroup>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularCalculationGroup"))
        {
            var row = new global::MetaTabular.TabularCalculationGroup
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Precedence = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Precedence");
            typed.TabularCalculationGroupList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularCalculationItem>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularCalculationItem"))
        {
            var row = new global::MetaTabular.TabularCalculationItem
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Expression = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Expression");
            row.FormatStringExpression = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "FormatStringExpression");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.TabularCalculationItemList.Add(row);
            index1.Add(source.Id, row);
        }

        var index2 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularColumn>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularColumn"))
        {
            var row = new global::MetaTabular.TabularColumn
            {
                Id = source.Id
            };
            row.DataCategory = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DataCategory");
            row.DataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DataTypeId");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Expression = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Expression");
            row.FormatString = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "FormatString");
            row.IsHidden = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsHidden");
            row.IsKey = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsKey");
            row.IsNullable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsNullable");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            row.SourceName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "SourceName");
            row.SummarizeBy = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "SummarizeBy");
            typed.TabularColumnList.Add(row);
            index2.Add(source.Id, row);
        }

        var index3 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularColumnPermission>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularColumnPermission"))
        {
            var row = new global::MetaTabular.TabularColumnPermission
            {
                Id = source.Id
            };
            row.MetadataPermission = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetadataPermission");
            typed.TabularColumnPermissionList.Add(row);
            index3.Add(source.Id, row);
        }

        var index4 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularColumnTranslation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularColumnTranslation"))
        {
            var row = new global::MetaTabular.TabularColumnTranslation
            {
                Id = source.Id
            };
            row.Caption = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Caption");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.TabularColumnTranslationList.Add(row);
            index4.Add(source.Id, row);
        }

        var index5 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularCulture>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularCulture"))
        {
            var row = new global::MetaTabular.TabularCulture
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.TabularCultureList.Add(row);
            index5.Add(source.Id, row);
        }

        var index6 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularDataSource>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularDataSource"))
        {
            var row = new global::MetaTabular.TabularDataSource
            {
                Id = source.Id
            };
            row.ConnectionReference = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "ConnectionReference");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Provider = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Provider");
            typed.TabularDataSourceList.Add(row);
            index6.Add(source.Id, row);
        }

        var index7 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularHierarchy>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularHierarchy"))
        {
            var row = new global::MetaTabular.TabularHierarchy
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.DisplayFolder = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DisplayFolder");
            row.IsHidden = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsHidden");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.TabularHierarchyList.Add(row);
            index7.Add(source.Id, row);
        }

        var index8 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularHierarchyLevel>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularHierarchyLevel"))
        {
            var row = new global::MetaTabular.TabularHierarchyLevel
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.TabularHierarchyLevelList.Add(row);
            index8.Add(source.Id, row);
        }

        var index9 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularHierarchyTranslation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularHierarchyTranslation"))
        {
            var row = new global::MetaTabular.TabularHierarchyTranslation
            {
                Id = source.Id
            };
            row.Caption = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Caption");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.TabularHierarchyTranslationList.Add(row);
            index9.Add(source.Id, row);
        }

        var index10 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularKpi>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularKpi"))
        {
            var row = new global::MetaTabular.TabularKpi
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.StatusExpression = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "StatusExpression");
            row.StatusGraphic = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "StatusGraphic");
            row.TargetExpression = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "TargetExpression");
            row.TrendExpression = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "TrendExpression");
            row.TrendGraphic = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "TrendGraphic");
            typed.TabularKpiList.Add(row);
            index10.Add(source.Id, row);
        }

        var index11 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularKpiTranslation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularKpiTranslation"))
        {
            var row = new global::MetaTabular.TabularKpiTranslation
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.TabularKpiTranslationList.Add(row);
            index11.Add(source.Id, row);
        }

        var index12 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularMeasure>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularMeasure"))
        {
            var row = new global::MetaTabular.TabularMeasure
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.DisplayFolder = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DisplayFolder");
            row.Expression = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Expression");
            row.FormatString = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "FormatString");
            row.IsHidden = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsHidden");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.TabularMeasureList.Add(row);
            index12.Add(source.Id, row);
        }

        var index13 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularMeasureTranslation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularMeasureTranslation"))
        {
            var row = new global::MetaTabular.TabularMeasureTranslation
            {
                Id = source.Id
            };
            row.Caption = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Caption");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.TabularMeasureTranslationList.Add(row);
            index13.Add(source.Id, row);
        }

        var index14 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularModel>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularModel"))
        {
            var row = new global::MetaTabular.TabularModel
            {
                Id = source.Id
            };
            row.Collation = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Collation");
            row.CompatibilityLevel = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "CompatibilityLevel");
            row.DefaultCulture = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DefaultCulture");
            row.DefaultDataView = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DefaultDataView");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.TabularModelList.Add(row);
            index14.Add(source.Id, row);
        }

        var index15 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularPartition>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPartition"))
        {
            var row = new global::MetaTabular.TabularPartition
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Expression = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Expression");
            row.Mode = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Mode");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.TabularPartitionList.Add(row);
            index15.Add(source.Id, row);
        }

        var index16 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularPerspective>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspective"))
        {
            var row = new global::MetaTabular.TabularPerspective
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.TabularPerspectiveList.Add(row);
            index16.Add(source.Id, row);
        }

        var index17 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularPerspectiveCalculationGroup>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspectiveCalculationGroup"))
        {
            var row = new global::MetaTabular.TabularPerspectiveCalculationGroup
            {
                Id = source.Id
            };
            typed.TabularPerspectiveCalculationGroupList.Add(row);
            index17.Add(source.Id, row);
        }

        var index18 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularPerspectiveColumn>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspectiveColumn"))
        {
            var row = new global::MetaTabular.TabularPerspectiveColumn
            {
                Id = source.Id
            };
            typed.TabularPerspectiveColumnList.Add(row);
            index18.Add(source.Id, row);
        }

        var index19 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularPerspectiveHierarchy>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspectiveHierarchy"))
        {
            var row = new global::MetaTabular.TabularPerspectiveHierarchy
            {
                Id = source.Id
            };
            typed.TabularPerspectiveHierarchyList.Add(row);
            index19.Add(source.Id, row);
        }

        var index20 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularPerspectiveKpi>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspectiveKpi"))
        {
            var row = new global::MetaTabular.TabularPerspectiveKpi
            {
                Id = source.Id
            };
            typed.TabularPerspectiveKpiList.Add(row);
            index20.Add(source.Id, row);
        }

        var index21 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularPerspectiveMeasure>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspectiveMeasure"))
        {
            var row = new global::MetaTabular.TabularPerspectiveMeasure
            {
                Id = source.Id
            };
            typed.TabularPerspectiveMeasureList.Add(row);
            index21.Add(source.Id, row);
        }

        var index22 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularPerspectiveTable>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspectiveTable"))
        {
            var row = new global::MetaTabular.TabularPerspectiveTable
            {
                Id = source.Id
            };
            typed.TabularPerspectiveTableList.Add(row);
            index22.Add(source.Id, row);
        }

        var index23 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularPerspectiveTranslation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspectiveTranslation"))
        {
            var row = new global::MetaTabular.TabularPerspectiveTranslation
            {
                Id = source.Id
            };
            row.Caption = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Caption");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.TabularPerspectiveTranslationList.Add(row);
            index23.Add(source.Id, row);
        }

        var index24 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularRelationship>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularRelationship"))
        {
            var row = new global::MetaTabular.TabularRelationship
            {
                Id = source.Id
            };
            row.Cardinality = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Cardinality");
            row.CrossFilterDirection = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "CrossFilterDirection");
            row.IsActive = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsActive");
            row.IsRequired = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsRequired");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.TabularRelationshipList.Add(row);
            index24.Add(source.Id, row);
        }

        var index25 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularRoleFilter>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularRoleFilter"))
        {
            var row = new global::MetaTabular.TabularRoleFilter
            {
                Id = source.Id
            };
            row.Expression = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Expression");
            typed.TabularRoleFilterList.Add(row);
            index25.Add(source.Id, row);
        }

        var index26 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularRoleMember>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularRoleMember"))
        {
            var row = new global::MetaTabular.TabularRoleMember
            {
                Id = source.Id
            };
            row.MemberId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "MemberId");
            row.MemberName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MemberName");
            typed.TabularRoleMemberList.Add(row);
            index26.Add(source.Id, row);
        }

        var index27 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularSecurityRole>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularSecurityRole"))
        {
            var row = new global::MetaTabular.TabularSecurityRole
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Permission = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Permission");
            typed.TabularSecurityRoleList.Add(row);
            index27.Add(source.Id, row);
        }

        var index28 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularSortByColumn>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularSortByColumn"))
        {
            var row = new global::MetaTabular.TabularSortByColumn
            {
                Id = source.Id
            };
            typed.TabularSortByColumnList.Add(row);
            index28.Add(source.Id, row);
        }

        var index29 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularTable>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularTable"))
        {
            var row = new global::MetaTabular.TabularTable
            {
                Id = source.Id
            };
            row.DataCategory = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DataCategory");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.IsHidden = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsHidden");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.TabularTableList.Add(row);
            index29.Add(source.Id, row);
        }

        var index30 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularTablePermission>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularTablePermission"))
        {
            var row = new global::MetaTabular.TabularTablePermission
            {
                Id = source.Id
            };
            row.MetadataPermission = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetadataPermission");
            typed.TabularTablePermissionList.Add(row);
            index30.Add(source.Id, row);
        }

        var index31 = new global::System.Collections.Generic.Dictionary<string, global::MetaTabular.TabularTableTranslation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularTableTranslation"))
        {
            var row = new global::MetaTabular.TabularTableTranslation
            {
                Id = source.Id
            };
            row.Caption = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Caption");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.TabularTableTranslationList.Add(row);
            index31.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularCalculationGroup"))
        {
            var row = index0[source.Id];
            row.TabularModel = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularModelId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularCalculationItem"))
        {
            var row = index1[source.Id];
            row.TabularCalculationGroup = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularCalculationGroupId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularColumn"))
        {
            var row = index2[source.Id];
            row.TabularTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularTableId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularColumnPermission"))
        {
            var row = index3[source.Id];
            row.TabularColumn = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularColumnId", index2);
            row.TabularSecurityRole = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularSecurityRoleId", index27);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularColumnTranslation"))
        {
            var row = index4[source.Id];
            row.TabularColumn = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularColumnId", index2);
            row.TabularCulture = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularCultureId", index5);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularCulture"))
        {
            var row = index5[source.Id];
            row.TabularModel = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularModelId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularDataSource"))
        {
            var row = index6[source.Id];
            row.TabularModel = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularModelId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularHierarchy"))
        {
            var row = index7[source.Id];
            row.TabularTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularTableId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularHierarchyLevel"))
        {
            var row = index8[source.Id];
            row.TabularColumn = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularColumnId", index2);
            row.TabularHierarchy = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularHierarchyId", index7);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularHierarchyTranslation"))
        {
            var row = index9[source.Id];
            row.TabularCulture = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularCultureId", index5);
            row.TabularHierarchy = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularHierarchyId", index7);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularKpi"))
        {
            var row = index10[source.Id];
            row.BaseMeasure = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "BaseMeasureId", index12);
            row.TargetMeasure = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalTarget(source, "TargetMeasureId", index12);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularKpiTranslation"))
        {
            var row = index11[source.Id];
            row.TabularCulture = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularCultureId", index5);
            row.TabularKpi = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularKpiId", index10);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularMeasure"))
        {
            var row = index12[source.Id];
            row.TabularTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularTableId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularMeasureTranslation"))
        {
            var row = index13[source.Id];
            row.TabularCulture = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularCultureId", index5);
            row.TabularMeasure = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularMeasureId", index12);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPartition"))
        {
            var row = index15[source.Id];
            row.TabularDataSource = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalTarget(source, "TabularDataSourceId", index6);
            row.TabularTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularTableId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspective"))
        {
            var row = index16[source.Id];
            row.TabularModel = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularModelId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspectiveCalculationGroup"))
        {
            var row = index17[source.Id];
            row.TabularCalculationGroup = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularCalculationGroupId", index0);
            row.TabularPerspective = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularPerspectiveId", index16);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspectiveColumn"))
        {
            var row = index18[source.Id];
            row.TabularColumn = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularColumnId", index2);
            row.TabularPerspective = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularPerspectiveId", index16);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspectiveHierarchy"))
        {
            var row = index19[source.Id];
            row.TabularHierarchy = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularHierarchyId", index7);
            row.TabularPerspective = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularPerspectiveId", index16);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspectiveKpi"))
        {
            var row = index20[source.Id];
            row.TabularKpi = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularKpiId", index10);
            row.TabularPerspective = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularPerspectiveId", index16);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspectiveMeasure"))
        {
            var row = index21[source.Id];
            row.TabularMeasure = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularMeasureId", index12);
            row.TabularPerspective = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularPerspectiveId", index16);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspectiveTable"))
        {
            var row = index22[source.Id];
            row.TabularPerspective = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularPerspectiveId", index16);
            row.TabularTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularTableId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularPerspectiveTranslation"))
        {
            var row = index23[source.Id];
            row.TabularCulture = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularCultureId", index5);
            row.TabularPerspective = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularPerspectiveId", index16);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularRelationship"))
        {
            var row = index24[source.Id];
            row.FromColumn = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FromColumnId", index2);
            row.FromTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FromTableId", index29);
            row.ToColumn = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ToColumnId", index2);
            row.ToTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ToTableId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularRoleFilter"))
        {
            var row = index25[source.Id];
            row.TabularSecurityRole = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularSecurityRoleId", index27);
            row.TabularTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularTableId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularRoleMember"))
        {
            var row = index26[source.Id];
            row.TabularSecurityRole = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularSecurityRoleId", index27);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularSecurityRole"))
        {
            var row = index27[source.Id];
            row.TabularModel = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularModelId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularSortByColumn"))
        {
            var row = index28[source.Id];
            row.SortColumn = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SortColumnId", index2);
            row.SourceColumn = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SourceColumnId", index2);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularTable"))
        {
            var row = index29[source.Id];
            row.TabularModel = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularModelId", index14);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularTablePermission"))
        {
            var row = index30[source.Id];
            row.TabularSecurityRole = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularSecurityRoleId", index27);
            row.TabularTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularTableId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TabularTableTranslation"))
        {
            var row = index31[source.Id];
            row.TabularCulture = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularCultureId", index5);
            row.TabularTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TabularTableId", index29);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaTabular.MetaTabularModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularCalculationGroupList, row => row.Id, "TabularCalculationGroup");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularCalculationItemList, row => row.Id, "TabularCalculationItem");
        var index2 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularColumnList, row => row.Id, "TabularColumn");
        var index3 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularColumnPermissionList, row => row.Id, "TabularColumnPermission");
        var index4 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularColumnTranslationList, row => row.Id, "TabularColumnTranslation");
        var index5 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularCultureList, row => row.Id, "TabularCulture");
        var index6 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularDataSourceList, row => row.Id, "TabularDataSource");
        var index7 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularHierarchyList, row => row.Id, "TabularHierarchy");
        var index8 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularHierarchyLevelList, row => row.Id, "TabularHierarchyLevel");
        var index9 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularHierarchyTranslationList, row => row.Id, "TabularHierarchyTranslation");
        var index10 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularKpiList, row => row.Id, "TabularKpi");
        var index11 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularKpiTranslationList, row => row.Id, "TabularKpiTranslation");
        var index12 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularMeasureList, row => row.Id, "TabularMeasure");
        var index13 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularMeasureTranslationList, row => row.Id, "TabularMeasureTranslation");
        var index14 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularModelList, row => row.Id, "TabularModel");
        var index15 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularPartitionList, row => row.Id, "TabularPartition");
        var index16 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularPerspectiveList, row => row.Id, "TabularPerspective");
        var index17 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularPerspectiveCalculationGroupList, row => row.Id, "TabularPerspectiveCalculationGroup");
        var index18 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularPerspectiveColumnList, row => row.Id, "TabularPerspectiveColumn");
        var index19 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularPerspectiveHierarchyList, row => row.Id, "TabularPerspectiveHierarchy");
        var index20 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularPerspectiveKpiList, row => row.Id, "TabularPerspectiveKpi");
        var index21 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularPerspectiveMeasureList, row => row.Id, "TabularPerspectiveMeasure");
        var index22 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularPerspectiveTableList, row => row.Id, "TabularPerspectiveTable");
        var index23 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularPerspectiveTranslationList, row => row.Id, "TabularPerspectiveTranslation");
        var index24 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularRelationshipList, row => row.Id, "TabularRelationship");
        var index25 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularRoleFilterList, row => row.Id, "TabularRoleFilter");
        var index26 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularRoleMemberList, row => row.Id, "TabularRoleMember");
        var index27 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularSecurityRoleList, row => row.Id, "TabularSecurityRole");
        var index28 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularSortByColumnList, row => row.Id, "TabularSortByColumn");
        var index29 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularTableList, row => row.Id, "TabularTable");
        var index30 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularTablePermissionList, row => row.Id, "TabularTablePermission");
        var index31 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TabularTableTranslationList, row => row.Id, "TabularTableTranslation");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Precedence is not null)
                target.Values.Add("Precedence", row.Precedence);
            if (row.TabularModel is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularModel, row.TabularModel.Id, index14, "TabularCalculationGroup.TabularModel");
                target.RelationshipIds.Add("TabularModelId", row.TabularModel.Id);
            }

            instance.GetOrCreateEntityRecords("TabularCalculationGroup").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Expression is not null)
                target.Values.Add("Expression", row.Expression);
            if (row.FormatStringExpression is not null)
                target.Values.Add("FormatStringExpression", row.FormatStringExpression);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.TabularCalculationGroup is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularCalculationGroup, row.TabularCalculationGroup.Id, index0, "TabularCalculationItem.TabularCalculationGroup");
                target.RelationshipIds.Add("TabularCalculationGroupId", row.TabularCalculationGroup.Id);
            }

            instance.GetOrCreateEntityRecords("TabularCalculationItem").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index2))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DataCategory is not null)
                target.Values.Add("DataCategory", row.DataCategory);
            if (row.DataTypeId is not null)
                target.Values.Add("DataTypeId", row.DataTypeId);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Expression is not null)
                target.Values.Add("Expression", row.Expression);
            if (row.FormatString is not null)
                target.Values.Add("FormatString", row.FormatString);
            if (row.IsHidden is not null)
                target.Values.Add("IsHidden", row.IsHidden);
            if (row.IsKey is not null)
                target.Values.Add("IsKey", row.IsKey);
            if (row.IsNullable is not null)
                target.Values.Add("IsNullable", row.IsNullable);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.SourceName is not null)
                target.Values.Add("SourceName", row.SourceName);
            if (row.SummarizeBy is not null)
                target.Values.Add("SummarizeBy", row.SummarizeBy);
            if (row.TabularTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularTable, row.TabularTable.Id, index29, "TabularColumn.TabularTable");
                target.RelationshipIds.Add("TabularTableId", row.TabularTable.Id);
            }

            instance.GetOrCreateEntityRecords("TabularColumn").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index3))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetadataPermission is not null)
                target.Values.Add("MetadataPermission", row.MetadataPermission);
            if (row.TabularColumn is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularColumn, row.TabularColumn.Id, index2, "TabularColumnPermission.TabularColumn");
                target.RelationshipIds.Add("TabularColumnId", row.TabularColumn.Id);
            }

            if (row.TabularSecurityRole is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularSecurityRole, row.TabularSecurityRole.Id, index27, "TabularColumnPermission.TabularSecurityRole");
                target.RelationshipIds.Add("TabularSecurityRoleId", row.TabularSecurityRole.Id);
            }

            instance.GetOrCreateEntityRecords("TabularColumnPermission").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index4))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Caption is not null)
                target.Values.Add("Caption", row.Caption);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.TabularColumn is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularColumn, row.TabularColumn.Id, index2, "TabularColumnTranslation.TabularColumn");
                target.RelationshipIds.Add("TabularColumnId", row.TabularColumn.Id);
            }

            if (row.TabularCulture is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularCulture, row.TabularCulture.Id, index5, "TabularColumnTranslation.TabularCulture");
                target.RelationshipIds.Add("TabularCultureId", row.TabularCulture.Id);
            }

            instance.GetOrCreateEntityRecords("TabularColumnTranslation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index5))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.TabularModel is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularModel, row.TabularModel.Id, index14, "TabularCulture.TabularModel");
                target.RelationshipIds.Add("TabularModelId", row.TabularModel.Id);
            }

            instance.GetOrCreateEntityRecords("TabularCulture").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index6))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.ConnectionReference is not null)
                target.Values.Add("ConnectionReference", row.ConnectionReference);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Provider is not null)
                target.Values.Add("Provider", row.Provider);
            if (row.TabularModel is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularModel, row.TabularModel.Id, index14, "TabularDataSource.TabularModel");
                target.RelationshipIds.Add("TabularModelId", row.TabularModel.Id);
            }

            instance.GetOrCreateEntityRecords("TabularDataSource").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index7))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.DisplayFolder is not null)
                target.Values.Add("DisplayFolder", row.DisplayFolder);
            if (row.IsHidden is not null)
                target.Values.Add("IsHidden", row.IsHidden);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.TabularTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularTable, row.TabularTable.Id, index29, "TabularHierarchy.TabularTable");
                target.RelationshipIds.Add("TabularTableId", row.TabularTable.Id);
            }

            instance.GetOrCreateEntityRecords("TabularHierarchy").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index8))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.TabularColumn is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularColumn, row.TabularColumn.Id, index2, "TabularHierarchyLevel.TabularColumn");
                target.RelationshipIds.Add("TabularColumnId", row.TabularColumn.Id);
            }

            if (row.TabularHierarchy is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularHierarchy, row.TabularHierarchy.Id, index7, "TabularHierarchyLevel.TabularHierarchy");
                target.RelationshipIds.Add("TabularHierarchyId", row.TabularHierarchy.Id);
            }

            instance.GetOrCreateEntityRecords("TabularHierarchyLevel").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index9))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Caption is not null)
                target.Values.Add("Caption", row.Caption);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.TabularCulture is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularCulture, row.TabularCulture.Id, index5, "TabularHierarchyTranslation.TabularCulture");
                target.RelationshipIds.Add("TabularCultureId", row.TabularCulture.Id);
            }

            if (row.TabularHierarchy is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularHierarchy, row.TabularHierarchy.Id, index7, "TabularHierarchyTranslation.TabularHierarchy");
                target.RelationshipIds.Add("TabularHierarchyId", row.TabularHierarchy.Id);
            }

            instance.GetOrCreateEntityRecords("TabularHierarchyTranslation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index10))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.StatusExpression is not null)
                target.Values.Add("StatusExpression", row.StatusExpression);
            if (row.StatusGraphic is not null)
                target.Values.Add("StatusGraphic", row.StatusGraphic);
            if (row.TargetExpression is not null)
                target.Values.Add("TargetExpression", row.TargetExpression);
            if (row.TrendExpression is not null)
                target.Values.Add("TrendExpression", row.TrendExpression);
            if (row.TrendGraphic is not null)
                target.Values.Add("TrendGraphic", row.TrendGraphic);
            if (row.BaseMeasure is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.BaseMeasure, row.BaseMeasure.Id, index12, "TabularKpi.BaseMeasure");
                target.RelationshipIds.Add("BaseMeasureId", row.BaseMeasure.Id);
            }

            if (row.TargetMeasure is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TargetMeasure, row.TargetMeasure.Id, index12, "TabularKpi.TargetMeasure");
                target.RelationshipIds.Add("TargetMeasureId", row.TargetMeasure.Id);
            }

            instance.GetOrCreateEntityRecords("TabularKpi").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index11))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.TabularCulture is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularCulture, row.TabularCulture.Id, index5, "TabularKpiTranslation.TabularCulture");
                target.RelationshipIds.Add("TabularCultureId", row.TabularCulture.Id);
            }

            if (row.TabularKpi is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularKpi, row.TabularKpi.Id, index10, "TabularKpiTranslation.TabularKpi");
                target.RelationshipIds.Add("TabularKpiId", row.TabularKpi.Id);
            }

            instance.GetOrCreateEntityRecords("TabularKpiTranslation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index12))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.DisplayFolder is not null)
                target.Values.Add("DisplayFolder", row.DisplayFolder);
            if (row.Expression is not null)
                target.Values.Add("Expression", row.Expression);
            if (row.FormatString is not null)
                target.Values.Add("FormatString", row.FormatString);
            if (row.IsHidden is not null)
                target.Values.Add("IsHidden", row.IsHidden);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.TabularTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularTable, row.TabularTable.Id, index29, "TabularMeasure.TabularTable");
                target.RelationshipIds.Add("TabularTableId", row.TabularTable.Id);
            }

            instance.GetOrCreateEntityRecords("TabularMeasure").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index13))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Caption is not null)
                target.Values.Add("Caption", row.Caption);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.TabularCulture is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularCulture, row.TabularCulture.Id, index5, "TabularMeasureTranslation.TabularCulture");
                target.RelationshipIds.Add("TabularCultureId", row.TabularCulture.Id);
            }

            if (row.TabularMeasure is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularMeasure, row.TabularMeasure.Id, index12, "TabularMeasureTranslation.TabularMeasure");
                target.RelationshipIds.Add("TabularMeasureId", row.TabularMeasure.Id);
            }

            instance.GetOrCreateEntityRecords("TabularMeasureTranslation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index14))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Collation is not null)
                target.Values.Add("Collation", row.Collation);
            if (row.CompatibilityLevel is not null)
                target.Values.Add("CompatibilityLevel", row.CompatibilityLevel);
            if (row.DefaultCulture is not null)
                target.Values.Add("DefaultCulture", row.DefaultCulture);
            if (row.DefaultDataView is not null)
                target.Values.Add("DefaultDataView", row.DefaultDataView);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            instance.GetOrCreateEntityRecords("TabularModel").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index15))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Expression is not null)
                target.Values.Add("Expression", row.Expression);
            if (row.Mode is not null)
                target.Values.Add("Mode", row.Mode);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.TabularDataSource is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularDataSource, row.TabularDataSource.Id, index6, "TabularPartition.TabularDataSource");
                target.RelationshipIds.Add("TabularDataSourceId", row.TabularDataSource.Id);
            }

            if (row.TabularTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularTable, row.TabularTable.Id, index29, "TabularPartition.TabularTable");
                target.RelationshipIds.Add("TabularTableId", row.TabularTable.Id);
            }

            instance.GetOrCreateEntityRecords("TabularPartition").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index16))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.TabularModel is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularModel, row.TabularModel.Id, index14, "TabularPerspective.TabularModel");
                target.RelationshipIds.Add("TabularModelId", row.TabularModel.Id);
            }

            instance.GetOrCreateEntityRecords("TabularPerspective").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index17))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.TabularCalculationGroup is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularCalculationGroup, row.TabularCalculationGroup.Id, index0, "TabularPerspectiveCalculationGroup.TabularCalculationGroup");
                target.RelationshipIds.Add("TabularCalculationGroupId", row.TabularCalculationGroup.Id);
            }

            if (row.TabularPerspective is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularPerspective, row.TabularPerspective.Id, index16, "TabularPerspectiveCalculationGroup.TabularPerspective");
                target.RelationshipIds.Add("TabularPerspectiveId", row.TabularPerspective.Id);
            }

            instance.GetOrCreateEntityRecords("TabularPerspectiveCalculationGroup").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index18))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.TabularColumn is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularColumn, row.TabularColumn.Id, index2, "TabularPerspectiveColumn.TabularColumn");
                target.RelationshipIds.Add("TabularColumnId", row.TabularColumn.Id);
            }

            if (row.TabularPerspective is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularPerspective, row.TabularPerspective.Id, index16, "TabularPerspectiveColumn.TabularPerspective");
                target.RelationshipIds.Add("TabularPerspectiveId", row.TabularPerspective.Id);
            }

            instance.GetOrCreateEntityRecords("TabularPerspectiveColumn").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index19))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.TabularHierarchy is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularHierarchy, row.TabularHierarchy.Id, index7, "TabularPerspectiveHierarchy.TabularHierarchy");
                target.RelationshipIds.Add("TabularHierarchyId", row.TabularHierarchy.Id);
            }

            if (row.TabularPerspective is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularPerspective, row.TabularPerspective.Id, index16, "TabularPerspectiveHierarchy.TabularPerspective");
                target.RelationshipIds.Add("TabularPerspectiveId", row.TabularPerspective.Id);
            }

            instance.GetOrCreateEntityRecords("TabularPerspectiveHierarchy").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index20))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.TabularKpi is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularKpi, row.TabularKpi.Id, index10, "TabularPerspectiveKpi.TabularKpi");
                target.RelationshipIds.Add("TabularKpiId", row.TabularKpi.Id);
            }

            if (row.TabularPerspective is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularPerspective, row.TabularPerspective.Id, index16, "TabularPerspectiveKpi.TabularPerspective");
                target.RelationshipIds.Add("TabularPerspectiveId", row.TabularPerspective.Id);
            }

            instance.GetOrCreateEntityRecords("TabularPerspectiveKpi").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index21))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.TabularMeasure is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularMeasure, row.TabularMeasure.Id, index12, "TabularPerspectiveMeasure.TabularMeasure");
                target.RelationshipIds.Add("TabularMeasureId", row.TabularMeasure.Id);
            }

            if (row.TabularPerspective is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularPerspective, row.TabularPerspective.Id, index16, "TabularPerspectiveMeasure.TabularPerspective");
                target.RelationshipIds.Add("TabularPerspectiveId", row.TabularPerspective.Id);
            }

            instance.GetOrCreateEntityRecords("TabularPerspectiveMeasure").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index22))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.TabularPerspective is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularPerspective, row.TabularPerspective.Id, index16, "TabularPerspectiveTable.TabularPerspective");
                target.RelationshipIds.Add("TabularPerspectiveId", row.TabularPerspective.Id);
            }

            if (row.TabularTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularTable, row.TabularTable.Id, index29, "TabularPerspectiveTable.TabularTable");
                target.RelationshipIds.Add("TabularTableId", row.TabularTable.Id);
            }

            instance.GetOrCreateEntityRecords("TabularPerspectiveTable").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index23))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Caption is not null)
                target.Values.Add("Caption", row.Caption);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.TabularCulture is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularCulture, row.TabularCulture.Id, index5, "TabularPerspectiveTranslation.TabularCulture");
                target.RelationshipIds.Add("TabularCultureId", row.TabularCulture.Id);
            }

            if (row.TabularPerspective is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularPerspective, row.TabularPerspective.Id, index16, "TabularPerspectiveTranslation.TabularPerspective");
                target.RelationshipIds.Add("TabularPerspectiveId", row.TabularPerspective.Id);
            }

            instance.GetOrCreateEntityRecords("TabularPerspectiveTranslation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index24))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Cardinality is not null)
                target.Values.Add("Cardinality", row.Cardinality);
            if (row.CrossFilterDirection is not null)
                target.Values.Add("CrossFilterDirection", row.CrossFilterDirection);
            if (row.IsActive is not null)
                target.Values.Add("IsActive", row.IsActive);
            if (row.IsRequired is not null)
                target.Values.Add("IsRequired", row.IsRequired);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.FromColumn is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.FromColumn, row.FromColumn.Id, index2, "TabularRelationship.FromColumn");
                target.RelationshipIds.Add("FromColumnId", row.FromColumn.Id);
            }

            if (row.FromTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.FromTable, row.FromTable.Id, index29, "TabularRelationship.FromTable");
                target.RelationshipIds.Add("FromTableId", row.FromTable.Id);
            }

            if (row.ToColumn is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ToColumn, row.ToColumn.Id, index2, "TabularRelationship.ToColumn");
                target.RelationshipIds.Add("ToColumnId", row.ToColumn.Id);
            }

            if (row.ToTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ToTable, row.ToTable.Id, index29, "TabularRelationship.ToTable");
                target.RelationshipIds.Add("ToTableId", row.ToTable.Id);
            }

            instance.GetOrCreateEntityRecords("TabularRelationship").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index25))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Expression is not null)
                target.Values.Add("Expression", row.Expression);
            if (row.TabularSecurityRole is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularSecurityRole, row.TabularSecurityRole.Id, index27, "TabularRoleFilter.TabularSecurityRole");
                target.RelationshipIds.Add("TabularSecurityRoleId", row.TabularSecurityRole.Id);
            }

            if (row.TabularTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularTable, row.TabularTable.Id, index29, "TabularRoleFilter.TabularTable");
                target.RelationshipIds.Add("TabularTableId", row.TabularTable.Id);
            }

            instance.GetOrCreateEntityRecords("TabularRoleFilter").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index26))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MemberId is not null)
                target.Values.Add("MemberId", row.MemberId);
            if (row.MemberName is not null)
                target.Values.Add("MemberName", row.MemberName);
            if (row.TabularSecurityRole is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularSecurityRole, row.TabularSecurityRole.Id, index27, "TabularRoleMember.TabularSecurityRole");
                target.RelationshipIds.Add("TabularSecurityRoleId", row.TabularSecurityRole.Id);
            }

            instance.GetOrCreateEntityRecords("TabularRoleMember").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index27))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Permission is not null)
                target.Values.Add("Permission", row.Permission);
            if (row.TabularModel is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularModel, row.TabularModel.Id, index14, "TabularSecurityRole.TabularModel");
                target.RelationshipIds.Add("TabularModelId", row.TabularModel.Id);
            }

            instance.GetOrCreateEntityRecords("TabularSecurityRole").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index28))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.SortColumn is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SortColumn, row.SortColumn.Id, index2, "TabularSortByColumn.SortColumn");
                target.RelationshipIds.Add("SortColumnId", row.SortColumn.Id);
            }

            if (row.SourceColumn is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SourceColumn, row.SourceColumn.Id, index2, "TabularSortByColumn.SourceColumn");
                target.RelationshipIds.Add("SourceColumnId", row.SourceColumn.Id);
            }

            instance.GetOrCreateEntityRecords("TabularSortByColumn").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index29))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DataCategory is not null)
                target.Values.Add("DataCategory", row.DataCategory);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.IsHidden is not null)
                target.Values.Add("IsHidden", row.IsHidden);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.TabularModel is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularModel, row.TabularModel.Id, index14, "TabularTable.TabularModel");
                target.RelationshipIds.Add("TabularModelId", row.TabularModel.Id);
            }

            instance.GetOrCreateEntityRecords("TabularTable").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index30))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetadataPermission is not null)
                target.Values.Add("MetadataPermission", row.MetadataPermission);
            if (row.TabularSecurityRole is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularSecurityRole, row.TabularSecurityRole.Id, index27, "TabularTablePermission.TabularSecurityRole");
                target.RelationshipIds.Add("TabularSecurityRoleId", row.TabularSecurityRole.Id);
            }

            if (row.TabularTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularTable, row.TabularTable.Id, index29, "TabularTablePermission.TabularTable");
                target.RelationshipIds.Add("TabularTableId", row.TabularTable.Id);
            }

            instance.GetOrCreateEntityRecords("TabularTablePermission").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index31))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Caption is not null)
                target.Values.Add("Caption", row.Caption);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.TabularCulture is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularCulture, row.TabularCulture.Id, index5, "TabularTableTranslation.TabularCulture");
                target.RelationshipIds.Add("TabularCultureId", row.TabularCulture.Id);
            }

            if (row.TabularTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TabularTable, row.TabularTable.Id, index29, "TabularTableTranslation.TabularTable");
                target.RelationshipIds.Add("TabularTableId", row.TabularTable.Id);
            }

            instance.GetOrCreateEntityRecords("TabularTableTranslation").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}