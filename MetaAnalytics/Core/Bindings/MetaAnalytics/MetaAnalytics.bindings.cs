// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaAnalytics;
public sealed class MetaAnalyticsWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaAnalytics.MetaAnalyticsModel>
{
    private const string ContractSignature = "6d6f64656c7c4d657461416e616c79746963730a656e746974797c41676772656761746546756e6374696f6e7c41676772656761746546756e6374696f6e4c6973740a656e746974797c416e616c79746963734d6f64656c7c416e616c79746963734d6f64656c4c6973740a70726f70657274797c416e616c79746963734d6f64656c7c44656661756c7443756c747572657c6e756c6c61626c650a70726f70657274797c416e616c79746963734d6f64656c7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c416e616c79746963734d6f64656c7c4e616d657c72657175697265640a656e746974797c4174747269627574657c4174747269627574654c6973740a70726f70657274797c4174747269627574657c4461746143617465676f72797c6e756c6c61626c650a70726f70657274797c4174747269627574657c446174615479706549647c72657175697265640a70726f70657274797c4174747269627574657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c4174747269627574657c466f726d6174537472696e677c6e756c6c61626c650a70726f70657274797c4174747269627574657c497348696464656e7c6e756c6c61626c650a70726f70657274797c4174747269627574657c49734b65797c6e756c6c61626c650a70726f70657274797c4174747269627574657c49734e756c6c61626c657c6e756c6c61626c650a70726f70657274797c4174747269627574657c4b696e647c6e756c6c61626c650a70726f70657274797c4174747269627574657c4e616d657c72657175697265640a70726f70657274797c4174747269627574657c4f7264696e616c7c72657175697265640a70726f70657274797c4174747269627574657c536f757263654e616d657c6e756c6c61626c650a70726f70657274797c4174747269627574657c53756d6d6172697a6542797c6e756c6c61626c650a72656c6174696f6e736869707c4174747269627574657c5461626c657c5461626c6549647c72657175697265640a656e746974797c4174747269627574655065726d697373696f6e7c4174747269627574655065726d697373696f6e4c6973740a70726f70657274797c4174747269627574655065726d697373696f6e7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c4174747269627574655065726d697373696f6e7c4d657461646174615065726d697373696f6e7c72657175697265640a72656c6174696f6e736869707c4174747269627574655065726d697373696f6e7c4174747269627574657c41747472696275746549647c72657175697265640a72656c6174696f6e736869707c4174747269627574655065726d697373696f6e7c5365637572697479526f6c657c5365637572697479526f6c6549647c72657175697265640a656e746974797c41747472696275746552656c6174696f6e736869707c41747472696275746552656c6174696f6e736869704c6973740a70726f70657274797c41747472696275746552656c6174696f6e736869707c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c41747472696275746552656c6174696f6e736869707c52656c6174696f6e73686970547970657c6e756c6c61626c650a72656c6174696f6e736869707c41747472696275746552656c6174696f6e736869707c4174747269627574657c4368696c6441747472696275746549647c72657175697265640a72656c6174696f6e736869707c41747472696275746552656c6174696f6e736869707c4174747269627574657c506172656e7441747472696275746549647c72657175697265640a656e746974797c4174747269627574655472616e736c6174696f6e7c4174747269627574655472616e736c6174696f6e4c6973740a70726f70657274797c4174747269627574655472616e736c6174696f6e7c43617074696f6e7c6e756c6c61626c650a70726f70657274797c4174747269627574655472616e736c6174696f6e7c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c4174747269627574655472616e736c6174696f6e7c4174747269627574657c41747472696275746549647c72657175697265640a72656c6174696f6e736869707c4174747269627574655472616e736c6174696f6e7c43756c747572657c43756c7475726549647c72657175697265640a656e746974797c4176657261676541676772656761746546756e6374696f6e7c4176657261676541676772656761746546756e6374696f6e4c6973740a72656c6174696f6e736869707c4176657261676541676772656761746546756e6374696f6e7c41676772656761746546756e6374696f6e7c41676772656761746546756e6374696f6e49647c72657175697265640a656e746974797c436f756e7441676772656761746546756e6374696f6e7c436f756e7441676772656761746546756e6374696f6e4c6973740a72656c6174696f6e736869707c436f756e7441676772656761746546756e6374696f6e7c41676772656761746546756e6374696f6e7c41676772656761746546756e6374696f6e49647c72657175697265640a656e746974797c43756c747572657c43756c747572654c6973740a70726f70657274797c43756c747572657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c43756c747572657c4e616d657c72657175697265640a72656c6174696f6e736869707c43756c747572657c416e616c79746963734d6f64656c7c416e616c79746963734d6f64656c49647c72657175697265640a656e746974797c44617461536f757263657c44617461536f757263654c6973740a70726f70657274797c44617461536f757263657c436f6e6e656374696f6e5265666572656e63657c6e756c6c61626c650a70726f70657274797c44617461536f757263657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c44617461536f757263657c4e616d657c72657175697265640a70726f70657274797c44617461536f757263657c50726f76696465727c6e756c6c61626c650a70726f70657274797c44617461536f757263657c536f757263654b696e647c6e756c6c61626c650a72656c6174696f6e736869707c44617461536f757263657c416e616c79746963734d6f64656c7c416e616c79746963734d6f64656c49647c72657175697265640a656e746974797c44697374696e6374436f756e7441676772656761746546756e6374696f6e7c44697374696e6374436f756e7441676772656761746546756e6374696f6e4c6973740a72656c6174696f6e736869707c44697374696e6374436f756e7441676772656761746546756e6374696f6e7c41676772656761746546756e6374696f6e7c41676772656761746546756e6374696f6e49647c72657175697265640a656e746974797c4869657261726368797c4869657261726368794c6973740a70726f70657274797c4869657261726368797c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c4869657261726368797c446973706c6179466f6c6465727c6e756c6c61626c650a70726f70657274797c4869657261726368797c497348696464656e7c6e756c6c61626c650a70726f70657274797c4869657261726368797c4b696e647c6e756c6c61626c650a70726f70657274797c4869657261726368797c4e616d657c72657175697265640a72656c6174696f6e736869707c4869657261726368797c5461626c657c5461626c6549647c72657175697265640a656e746974797c4869657261726368794c6576656c7c4869657261726368794c6576656c4c6973740a70726f70657274797c4869657261726368794c6576656c7c4e616d657c72657175697265640a70726f70657274797c4869657261726368794c6576656c7c4f7264696e616c7c72657175697265640a72656c6174696f6e736869707c4869657261726368794c6576656c7c4174747269627574657c41747472696275746549647c72657175697265640a72656c6174696f6e736869707c4869657261726368794c6576656c7c4869657261726368797c48696572617263687949647c72657175697265640a656e746974797c4869657261726368795472616e736c6174696f6e7c4869657261726368795472616e736c6174696f6e4c6973740a70726f70657274797c4869657261726368795472616e736c6174696f6e7c43617074696f6e7c6e756c6c61626c650a70726f70657274797c4869657261726368795472616e736c6174696f6e7c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c4869657261726368795472616e736c6174696f6e7c43756c747572657c43756c7475726549647c72657175697265640a72656c6174696f6e736869707c4869657261726368795472616e736c6174696f6e7c4869657261726368797c48696572617263687949647c72657175697265640a656e746974797c4d6178696d756d41676772656761746546756e6374696f6e7c4d6178696d756d41676772656761746546756e6374696f6e4c6973740a72656c6174696f6e736869707c4d6178696d756d41676772656761746546756e6374696f6e7c41676772656761746546756e6374696f6e7c41676772656761746546756e6374696f6e49647c72657175697265640a656e746974797c4d6561737572657c4d6561737572654c6973740a70726f70657274797c4d6561737572657c446174615479706549647c6e756c6c61626c650a70726f70657274797c4d6561737572657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c4d6561737572657c446973706c6179466f6c6465727c6e756c6c61626c650a70726f70657274797c4d6561737572657c466f726d6174537472696e677c6e756c6c61626c650a70726f70657274797c4d6561737572657c497348696464656e7c6e756c6c61626c650a70726f70657274797c4d6561737572657c4e616d657c72657175697265640a72656c6174696f6e736869707c4d6561737572657c41676772656761746546756e6374696f6e7c41676772656761746546756e6374696f6e49647c72657175697265640a72656c6174696f6e736869707c4d6561737572657c4174747269627574657c536f7572636541747472696275746549647c72657175697265640a72656c6174696f6e736869707c4d6561737572657c5461626c657c5461626c6549647c72657175697265640a656e746974797c4d6561737572655472616e736c6174696f6e7c4d6561737572655472616e736c6174696f6e4c6973740a70726f70657274797c4d6561737572655472616e736c6174696f6e7c43617074696f6e7c6e756c6c61626c650a70726f70657274797c4d6561737572655472616e736c6174696f6e7c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c4d6561737572655472616e736c6174696f6e7c43756c747572657c43756c7475726549647c72657175697265640a72656c6174696f6e736869707c4d6561737572655472616e736c6174696f6e7c4d6561737572657c4d65617375726549647c72657175697265640a656e746974797c4d696e696d756d41676772656761746546756e6374696f6e7c4d696e696d756d41676772656761746546756e6374696f6e4c6973740a72656c6174696f6e736869707c4d696e696d756d41676772656761746546756e6374696f6e7c41676772656761746546756e6374696f6e7c41676772656761746546756e6374696f6e49647c72657175697265640a656e746974797c50657273706563746976657c50657273706563746976654c6973740a70726f70657274797c50657273706563746976657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c50657273706563746976657c4e616d657c72657175697265640a72656c6174696f6e736869707c50657273706563746976657c416e616c79746963734d6f64656c7c416e616c79746963734d6f64656c49647c72657175697265640a656e746974797c50657273706563746976654174747269627574657c50657273706563746976654174747269627574654c6973740a72656c6174696f6e736869707c50657273706563746976654174747269627574657c4174747269627574657c41747472696275746549647c72657175697265640a72656c6174696f6e736869707c50657273706563746976654174747269627574657c50657273706563746976657c506572737065637469766549647c72657175697265640a656e746974797c50657273706563746976654869657261726368797c50657273706563746976654869657261726368794c6973740a72656c6174696f6e736869707c50657273706563746976654869657261726368797c4869657261726368797c48696572617263687949647c72657175697265640a72656c6174696f6e736869707c50657273706563746976654869657261726368797c50657273706563746976657c506572737065637469766549647c72657175697265640a656e746974797c50657273706563746976654d6561737572657c50657273706563746976654d6561737572654c6973740a72656c6174696f6e736869707c50657273706563746976654d6561737572657c4d6561737572657c4d65617375726549647c72657175697265640a72656c6174696f6e736869707c50657273706563746976654d6561737572657c50657273706563746976657c506572737065637469766549647c72657175697265640a656e746974797c50657273706563746976655461626c657c50657273706563746976655461626c654c6973740a72656c6174696f6e736869707c50657273706563746976655461626c657c50657273706563746976657c506572737065637469766549647c72657175697265640a72656c6174696f6e736869707c50657273706563746976655461626c657c5461626c657c5461626c6549647c72657175697265640a656e746974797c50657273706563746976655472616e736c6174696f6e7c50657273706563746976655472616e736c6174696f6e4c6973740a70726f70657274797c50657273706563746976655472616e736c6174696f6e7c43617074696f6e7c6e756c6c61626c650a70726f70657274797c50657273706563746976655472616e736c6174696f6e7c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c50657273706563746976655472616e736c6174696f6e7c43756c747572657c43756c7475726549647c72657175697265640a72656c6174696f6e736869707c50657273706563746976655472616e736c6174696f6e7c50657273706563746976657c506572737065637469766549647c72657175697265640a656e746974797c52656c6174696f6e736869707c52656c6174696f6e736869704c6973740a70726f70657274797c52656c6174696f6e736869707c43617264696e616c6974797c72657175697265640a70726f70657274797c52656c6174696f6e736869707c43726f737346696c746572446972656374696f6e7c6e756c6c61626c650a70726f70657274797c52656c6174696f6e736869707c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c52656c6174696f6e736869707c49734163746976657c6e756c6c61626c650a70726f70657274797c52656c6174696f6e736869707c497352657175697265647c6e756c6c61626c650a70726f70657274797c52656c6174696f6e736869707c4e616d657c72657175697265640a70726f70657274797c52656c6174696f6e736869707c52656c6174696f6e736869704b696e647c72657175697265640a70726f70657274797c52656c6174696f6e736869707c526f6c654e616d657c6e756c6c61626c650a72656c6174696f6e736869707c52656c6174696f6e736869707c4174747269627574657c46726f6d41747472696275746549647c72657175697265640a72656c6174696f6e736869707c52656c6174696f6e736869707c5461626c657c46726f6d5461626c6549647c72657175697265640a72656c6174696f6e736869707c52656c6174696f6e736869707c4174747269627574657c4772616e756c617269747941747472696275746549647c6e756c6c61626c650a72656c6174696f6e736869707c52656c6174696f6e736869707c5461626c657c496e7465726d6564696174655461626c6549647c6e756c6c61626c650a72656c6174696f6e736869707c52656c6174696f6e736869707c4174747269627574657c546f41747472696275746549647c72657175697265640a72656c6174696f6e736869707c52656c6174696f6e736869707c5461626c657c546f5461626c6549647c72657175697265640a656e746974797c526f6c654d656d6265727c526f6c654d656d6265724c6973740a70726f70657274797c526f6c654d656d6265727c4d656d6265724b696e647c6e756c6c61626c650a70726f70657274797c526f6c654d656d6265727c4d656d6265724e616d657c72657175697265640a72656c6174696f6e736869707c526f6c654d656d6265727c5365637572697479526f6c657c5365637572697479526f6c6549647c72657175697265640a656e746974797c5365637572697479526f6c657c5365637572697479526f6c654c6973740a70726f70657274797c5365637572697479526f6c657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c5365637572697479526f6c657c4e616d657c72657175697265640a70726f70657274797c5365637572697479526f6c657c5065726d697373696f6e7c72657175697265640a72656c6174696f6e736869707c5365637572697479526f6c657c416e616c79746963734d6f64656c7c416e616c79746963734d6f64656c49647c72657175697265640a656e746974797c536f727442794174747269627574657c536f727442794174747269627574654c6973740a70726f70657274797c536f727442794174747269627574657c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c536f727442794174747269627574657c4174747269627574657c536f727441747472696275746549647c72657175697265640a72656c6174696f6e736869707c536f727442794174747269627574657c4174747269627574657c536f7572636541747472696275746549647c72657175697265640a656e746974797c53756d41676772656761746546756e6374696f6e7c53756d41676772656761746546756e6374696f6e4c6973740a72656c6174696f6e736869707c53756d41676772656761746546756e6374696f6e7c41676772656761746546756e6374696f6e7c41676772656761746546756e6374696f6e49647c72657175697265640a656e746974797c5461626c657c5461626c654c6973740a70726f70657274797c5461626c657c4461746143617465676f72797c6e756c6c61626c650a70726f70657274797c5461626c657c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c5461626c657c446973706c6179466f6c6465727c6e756c6c61626c650a70726f70657274797c5461626c657c497348696464656e7c6e756c6c61626c650a70726f70657274797c5461626c657c4b696e647c72657175697265640a70726f70657274797c5461626c657c4e616d657c72657175697265640a72656c6174696f6e736869707c5461626c657c416e616c79746963734d6f64656c7c416e616c79746963734d6f64656c49647c72657175697265640a656e746974797c5461626c655065726d697373696f6e7c5461626c655065726d697373696f6e4c6973740a70726f70657274797c5461626c655065726d697373696f6e7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c5461626c655065726d697373696f6e7c4d657461646174615065726d697373696f6e7c72657175697265640a72656c6174696f6e736869707c5461626c655065726d697373696f6e7c5365637572697479526f6c657c5365637572697479526f6c6549647c72657175697265640a72656c6174696f6e736869707c5461626c655065726d697373696f6e7c5461626c657c5461626c6549647c72657175697265640a656e746974797c5461626c655472616e736c6174696f6e7c5461626c655472616e736c6174696f6e4c6973740a70726f70657274797c5461626c655472616e736c6174696f6e7c43617074696f6e7c6e756c6c61626c650a70726f70657274797c5461626c655472616e736c6174696f6e7c4465736372697074696f6e7c6e756c6c61626c650a72656c6174696f6e736869707c5461626c655472616e736c6174696f6e7c43756c747572657c43756c7475726549647c72657175697265640a72656c6174696f6e736869707c5461626c655472616e736c6174696f6e7c5461626c657c5461626c6549647c7265717569726564";
    public global::MetaAnalytics.MetaAnalyticsModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaAnalytics.MetaAnalyticsModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.AggregateFunction>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AggregateFunction"))
        {
            var row = new global::MetaAnalytics.AggregateFunction
            {
                Id = source.Id
            };
            typed.AggregateFunctionList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.AnalyticsModel>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AnalyticsModel"))
        {
            var row = new global::MetaAnalytics.AnalyticsModel
            {
                Id = source.Id
            };
            row.DefaultCulture = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DefaultCulture");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.AnalyticsModelList.Add(row);
            index1.Add(source.Id, row);
        }

        var index2 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.Attribute>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Attribute"))
        {
            var row = new global::MetaAnalytics.Attribute
            {
                Id = source.Id
            };
            row.DataCategory = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DataCategory");
            row.DataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DataTypeId");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.FormatString = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "FormatString");
            row.IsHidden = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsHidden");
            row.IsKey = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsKey");
            row.IsNullable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsNullable");
            row.Kind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Kind");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            row.SourceName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "SourceName");
            row.SummarizeBy = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "SummarizeBy");
            typed.AttributeList.Add(row);
            index2.Add(source.Id, row);
        }

        var index3 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.AttributePermission>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AttributePermission"))
        {
            var row = new global::MetaAnalytics.AttributePermission
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.MetadataPermission = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetadataPermission");
            typed.AttributePermissionList.Add(row);
            index3.Add(source.Id, row);
        }

        var index4 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.AttributeRelationship>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AttributeRelationship"))
        {
            var row = new global::MetaAnalytics.AttributeRelationship
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.RelationshipType = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "RelationshipType");
            typed.AttributeRelationshipList.Add(row);
            index4.Add(source.Id, row);
        }

        var index5 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.AttributeTranslation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AttributeTranslation"))
        {
            var row = new global::MetaAnalytics.AttributeTranslation
            {
                Id = source.Id
            };
            row.Caption = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Caption");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.AttributeTranslationList.Add(row);
            index5.Add(source.Id, row);
        }

        var index6 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.AverageAggregateFunction>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AverageAggregateFunction"))
        {
            var row = new global::MetaAnalytics.AverageAggregateFunction
            {
                Id = source.Id
            };
            typed.AverageAggregateFunctionList.Add(row);
            index6.Add(source.Id, row);
        }

        var index7 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.CountAggregateFunction>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "CountAggregateFunction"))
        {
            var row = new global::MetaAnalytics.CountAggregateFunction
            {
                Id = source.Id
            };
            typed.CountAggregateFunctionList.Add(row);
            index7.Add(source.Id, row);
        }

        var index8 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.Culture>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Culture"))
        {
            var row = new global::MetaAnalytics.Culture
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.CultureList.Add(row);
            index8.Add(source.Id, row);
        }

        var index9 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.DataSource>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DataSource"))
        {
            var row = new global::MetaAnalytics.DataSource
            {
                Id = source.Id
            };
            row.ConnectionReference = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "ConnectionReference");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Provider = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Provider");
            row.SourceKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "SourceKind");
            typed.DataSourceList.Add(row);
            index9.Add(source.Id, row);
        }

        var index10 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.DistinctCountAggregateFunction>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DistinctCountAggregateFunction"))
        {
            var row = new global::MetaAnalytics.DistinctCountAggregateFunction
            {
                Id = source.Id
            };
            typed.DistinctCountAggregateFunctionList.Add(row);
            index10.Add(source.Id, row);
        }

        var index11 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.Hierarchy>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Hierarchy"))
        {
            var row = new global::MetaAnalytics.Hierarchy
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.DisplayFolder = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DisplayFolder");
            row.IsHidden = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsHidden");
            row.Kind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Kind");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.HierarchyList.Add(row);
            index11.Add(source.Id, row);
        }

        var index12 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.HierarchyLevel>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "HierarchyLevel"))
        {
            var row = new global::MetaAnalytics.HierarchyLevel
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            typed.HierarchyLevelList.Add(row);
            index12.Add(source.Id, row);
        }

        var index13 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.HierarchyTranslation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "HierarchyTranslation"))
        {
            var row = new global::MetaAnalytics.HierarchyTranslation
            {
                Id = source.Id
            };
            row.Caption = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Caption");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.HierarchyTranslationList.Add(row);
            index13.Add(source.Id, row);
        }

        var index14 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.MaximumAggregateFunction>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "MaximumAggregateFunction"))
        {
            var row = new global::MetaAnalytics.MaximumAggregateFunction
            {
                Id = source.Id
            };
            typed.MaximumAggregateFunctionList.Add(row);
            index14.Add(source.Id, row);
        }

        var index15 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.Measure>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Measure"))
        {
            var row = new global::MetaAnalytics.Measure
            {
                Id = source.Id
            };
            row.DataTypeId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DataTypeId");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.DisplayFolder = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DisplayFolder");
            row.FormatString = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "FormatString");
            row.IsHidden = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsHidden");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.MeasureList.Add(row);
            index15.Add(source.Id, row);
        }

        var index16 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.MeasureTranslation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "MeasureTranslation"))
        {
            var row = new global::MetaAnalytics.MeasureTranslation
            {
                Id = source.Id
            };
            row.Caption = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Caption");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.MeasureTranslationList.Add(row);
            index16.Add(source.Id, row);
        }

        var index17 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.MinimumAggregateFunction>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "MinimumAggregateFunction"))
        {
            var row = new global::MetaAnalytics.MinimumAggregateFunction
            {
                Id = source.Id
            };
            typed.MinimumAggregateFunctionList.Add(row);
            index17.Add(source.Id, row);
        }

        var index18 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.Perspective>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Perspective"))
        {
            var row = new global::MetaAnalytics.Perspective
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.PerspectiveList.Add(row);
            index18.Add(source.Id, row);
        }

        var index19 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.PerspectiveAttribute>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PerspectiveAttribute"))
        {
            var row = new global::MetaAnalytics.PerspectiveAttribute
            {
                Id = source.Id
            };
            typed.PerspectiveAttributeList.Add(row);
            index19.Add(source.Id, row);
        }

        var index20 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.PerspectiveHierarchy>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PerspectiveHierarchy"))
        {
            var row = new global::MetaAnalytics.PerspectiveHierarchy
            {
                Id = source.Id
            };
            typed.PerspectiveHierarchyList.Add(row);
            index20.Add(source.Id, row);
        }

        var index21 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.PerspectiveMeasure>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PerspectiveMeasure"))
        {
            var row = new global::MetaAnalytics.PerspectiveMeasure
            {
                Id = source.Id
            };
            typed.PerspectiveMeasureList.Add(row);
            index21.Add(source.Id, row);
        }

        var index22 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.PerspectiveTable>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PerspectiveTable"))
        {
            var row = new global::MetaAnalytics.PerspectiveTable
            {
                Id = source.Id
            };
            typed.PerspectiveTableList.Add(row);
            index22.Add(source.Id, row);
        }

        var index23 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.PerspectiveTranslation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PerspectiveTranslation"))
        {
            var row = new global::MetaAnalytics.PerspectiveTranslation
            {
                Id = source.Id
            };
            row.Caption = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Caption");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.PerspectiveTranslationList.Add(row);
            index23.Add(source.Id, row);
        }

        var index24 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.Relationship>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Relationship"))
        {
            var row = new global::MetaAnalytics.Relationship
            {
                Id = source.Id
            };
            row.Cardinality = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Cardinality");
            row.CrossFilterDirection = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "CrossFilterDirection");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.IsActive = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsActive");
            row.IsRequired = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsRequired");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.RelationshipKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "RelationshipKind");
            row.RoleName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "RoleName");
            typed.RelationshipList.Add(row);
            index24.Add(source.Id, row);
        }

        var index25 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.RoleMember>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RoleMember"))
        {
            var row = new global::MetaAnalytics.RoleMember
            {
                Id = source.Id
            };
            row.MemberKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "MemberKind");
            row.MemberName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MemberName");
            typed.RoleMemberList.Add(row);
            index25.Add(source.Id, row);
        }

        var index26 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.SecurityRole>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SecurityRole"))
        {
            var row = new global::MetaAnalytics.SecurityRole
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Permission = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Permission");
            typed.SecurityRoleList.Add(row);
            index26.Add(source.Id, row);
        }

        var index27 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.SortByAttribute>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SortByAttribute"))
        {
            var row = new global::MetaAnalytics.SortByAttribute
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.SortByAttributeList.Add(row);
            index27.Add(source.Id, row);
        }

        var index28 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.SumAggregateFunction>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SumAggregateFunction"))
        {
            var row = new global::MetaAnalytics.SumAggregateFunction
            {
                Id = source.Id
            };
            typed.SumAggregateFunctionList.Add(row);
            index28.Add(source.Id, row);
        }

        var index29 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.Table>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Table"))
        {
            var row = new global::MetaAnalytics.Table
            {
                Id = source.Id
            };
            row.DataCategory = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DataCategory");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.DisplayFolder = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "DisplayFolder");
            row.IsHidden = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "IsHidden");
            row.Kind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Kind");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            typed.TableList.Add(row);
            index29.Add(source.Id, row);
        }

        var index30 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.TablePermission>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TablePermission"))
        {
            var row = new global::MetaAnalytics.TablePermission
            {
                Id = source.Id
            };
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.MetadataPermission = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetadataPermission");
            typed.TablePermissionList.Add(row);
            index30.Add(source.Id, row);
        }

        var index31 = new global::System.Collections.Generic.Dictionary<string, global::MetaAnalytics.TableTranslation>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TableTranslation"))
        {
            var row = new global::MetaAnalytics.TableTranslation
            {
                Id = source.Id
            };
            row.Caption = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Caption");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            typed.TableTranslationList.Add(row);
            index31.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Attribute"))
        {
            var row = index2[source.Id];
            row.Table = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TableId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AttributePermission"))
        {
            var row = index3[source.Id];
            row.Attribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AttributeId", index2);
            row.SecurityRole = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SecurityRoleId", index26);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AttributeRelationship"))
        {
            var row = index4[source.Id];
            row.ChildAttribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ChildAttributeId", index2);
            row.ParentAttribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ParentAttributeId", index2);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AttributeTranslation"))
        {
            var row = index5[source.Id];
            row.Attribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AttributeId", index2);
            row.Culture = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "CultureId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "AverageAggregateFunction"))
        {
            var row = index6[source.Id];
            row.AggregateFunction = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AggregateFunctionId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "CountAggregateFunction"))
        {
            var row = index7[source.Id];
            row.AggregateFunction = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AggregateFunctionId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Culture"))
        {
            var row = index8[source.Id];
            row.AnalyticsModel = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AnalyticsModelId", index1);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DataSource"))
        {
            var row = index9[source.Id];
            row.AnalyticsModel = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AnalyticsModelId", index1);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DistinctCountAggregateFunction"))
        {
            var row = index10[source.Id];
            row.AggregateFunction = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AggregateFunctionId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Hierarchy"))
        {
            var row = index11[source.Id];
            row.Table = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TableId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "HierarchyLevel"))
        {
            var row = index12[source.Id];
            row.Attribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AttributeId", index2);
            row.Hierarchy = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "HierarchyId", index11);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "HierarchyTranslation"))
        {
            var row = index13[source.Id];
            row.Culture = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "CultureId", index8);
            row.Hierarchy = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "HierarchyId", index11);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "MaximumAggregateFunction"))
        {
            var row = index14[source.Id];
            row.AggregateFunction = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AggregateFunctionId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Measure"))
        {
            var row = index15[source.Id];
            row.AggregateFunction = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AggregateFunctionId", index0);
            row.SourceAttribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SourceAttributeId", index2);
            row.Table = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TableId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "MeasureTranslation"))
        {
            var row = index16[source.Id];
            row.Culture = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "CultureId", index8);
            row.Measure = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "MeasureId", index15);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "MinimumAggregateFunction"))
        {
            var row = index17[source.Id];
            row.AggregateFunction = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AggregateFunctionId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Perspective"))
        {
            var row = index18[source.Id];
            row.AnalyticsModel = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AnalyticsModelId", index1);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PerspectiveAttribute"))
        {
            var row = index19[source.Id];
            row.Attribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AttributeId", index2);
            row.Perspective = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PerspectiveId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PerspectiveHierarchy"))
        {
            var row = index20[source.Id];
            row.Hierarchy = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "HierarchyId", index11);
            row.Perspective = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PerspectiveId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PerspectiveMeasure"))
        {
            var row = index21[source.Id];
            row.Measure = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "MeasureId", index15);
            row.Perspective = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PerspectiveId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PerspectiveTable"))
        {
            var row = index22[source.Id];
            row.Perspective = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PerspectiveId", index18);
            row.Table = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TableId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PerspectiveTranslation"))
        {
            var row = index23[source.Id];
            row.Culture = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "CultureId", index8);
            row.Perspective = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PerspectiveId", index18);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Relationship"))
        {
            var row = index24[source.Id];
            row.FromAttribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FromAttributeId", index2);
            row.FromTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "FromTableId", index29);
            row.GranularityAttribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalTarget(source, "GranularityAttributeId", index2);
            row.IntermediateTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalTarget(source, "IntermediateTableId", index29);
            row.ToAttribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ToAttributeId", index2);
            row.ToTable = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "ToTableId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RoleMember"))
        {
            var row = index25[source.Id];
            row.SecurityRole = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SecurityRoleId", index26);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SecurityRole"))
        {
            var row = index26[source.Id];
            row.AnalyticsModel = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AnalyticsModelId", index1);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SortByAttribute"))
        {
            var row = index27[source.Id];
            row.SortAttribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SortAttributeId", index2);
            row.SourceAttribute = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SourceAttributeId", index2);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "SumAggregateFunction"))
        {
            var row = index28[source.Id];
            row.AggregateFunction = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AggregateFunctionId", index0);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "Table"))
        {
            var row = index29[source.Id];
            row.AnalyticsModel = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "AnalyticsModelId", index1);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TablePermission"))
        {
            var row = index30[source.Id];
            row.SecurityRole = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SecurityRoleId", index26);
            row.Table = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TableId", index29);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TableTranslation"))
        {
            var row = index31[source.Id];
            row.Culture = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "CultureId", index8);
            row.Table = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TableId", index29);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaAnalytics.MetaAnalyticsModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AggregateFunctionList, row => row.Id, "AggregateFunction");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AnalyticsModelList, row => row.Id, "AnalyticsModel");
        var index2 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AttributeList, row => row.Id, "Attribute");
        var index3 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AttributePermissionList, row => row.Id, "AttributePermission");
        var index4 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AttributeRelationshipList, row => row.Id, "AttributeRelationship");
        var index5 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AttributeTranslationList, row => row.Id, "AttributeTranslation");
        var index6 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.AverageAggregateFunctionList, row => row.Id, "AverageAggregateFunction");
        var index7 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.CountAggregateFunctionList, row => row.Id, "CountAggregateFunction");
        var index8 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.CultureList, row => row.Id, "Culture");
        var index9 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DataSourceList, row => row.Id, "DataSource");
        var index10 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DistinctCountAggregateFunctionList, row => row.Id, "DistinctCountAggregateFunction");
        var index11 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.HierarchyList, row => row.Id, "Hierarchy");
        var index12 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.HierarchyLevelList, row => row.Id, "HierarchyLevel");
        var index13 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.HierarchyTranslationList, row => row.Id, "HierarchyTranslation");
        var index14 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.MaximumAggregateFunctionList, row => row.Id, "MaximumAggregateFunction");
        var index15 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.MeasureList, row => row.Id, "Measure");
        var index16 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.MeasureTranslationList, row => row.Id, "MeasureTranslation");
        var index17 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.MinimumAggregateFunctionList, row => row.Id, "MinimumAggregateFunction");
        var index18 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PerspectiveList, row => row.Id, "Perspective");
        var index19 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PerspectiveAttributeList, row => row.Id, "PerspectiveAttribute");
        var index20 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PerspectiveHierarchyList, row => row.Id, "PerspectiveHierarchy");
        var index21 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PerspectiveMeasureList, row => row.Id, "PerspectiveMeasure");
        var index22 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PerspectiveTableList, row => row.Id, "PerspectiveTable");
        var index23 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PerspectiveTranslationList, row => row.Id, "PerspectiveTranslation");
        var index24 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RelationshipList, row => row.Id, "Relationship");
        var index25 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RoleMemberList, row => row.Id, "RoleMember");
        var index26 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.SecurityRoleList, row => row.Id, "SecurityRole");
        var index27 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.SortByAttributeList, row => row.Id, "SortByAttribute");
        var index28 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.SumAggregateFunctionList, row => row.Id, "SumAggregateFunction");
        var index29 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TableList, row => row.Id, "Table");
        var index30 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TablePermissionList, row => row.Id, "TablePermission");
        var index31 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TableTranslationList, row => row.Id, "TableTranslation");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            instance.GetOrCreateEntityRecords("AggregateFunction").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DefaultCulture is not null)
                target.Values.Add("DefaultCulture", row.DefaultCulture);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            instance.GetOrCreateEntityRecords("AnalyticsModel").Add(target);
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
            if (row.FormatString is not null)
                target.Values.Add("FormatString", row.FormatString);
            if (row.IsHidden is not null)
                target.Values.Add("IsHidden", row.IsHidden);
            if (row.IsKey is not null)
                target.Values.Add("IsKey", row.IsKey);
            if (row.IsNullable is not null)
                target.Values.Add("IsNullable", row.IsNullable);
            if (row.Kind is not null)
                target.Values.Add("Kind", row.Kind);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.SourceName is not null)
                target.Values.Add("SourceName", row.SourceName);
            if (row.SummarizeBy is not null)
                target.Values.Add("SummarizeBy", row.SummarizeBy);
            if (row.Table is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Table, row.Table.Id, index29, "Attribute.Table");
                target.RelationshipIds.Add("TableId", row.Table.Id);
            }

            instance.GetOrCreateEntityRecords("Attribute").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index3))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.MetadataPermission is not null)
                target.Values.Add("MetadataPermission", row.MetadataPermission);
            if (row.Attribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Attribute, row.Attribute.Id, index2, "AttributePermission.Attribute");
                target.RelationshipIds.Add("AttributeId", row.Attribute.Id);
            }

            if (row.SecurityRole is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SecurityRole, row.SecurityRole.Id, index26, "AttributePermission.SecurityRole");
                target.RelationshipIds.Add("SecurityRoleId", row.SecurityRole.Id);
            }

            instance.GetOrCreateEntityRecords("AttributePermission").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index4))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.RelationshipType is not null)
                target.Values.Add("RelationshipType", row.RelationshipType);
            if (row.ChildAttribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ChildAttribute, row.ChildAttribute.Id, index2, "AttributeRelationship.ChildAttribute");
                target.RelationshipIds.Add("ChildAttributeId", row.ChildAttribute.Id);
            }

            if (row.ParentAttribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ParentAttribute, row.ParentAttribute.Id, index2, "AttributeRelationship.ParentAttribute");
                target.RelationshipIds.Add("ParentAttributeId", row.ParentAttribute.Id);
            }

            instance.GetOrCreateEntityRecords("AttributeRelationship").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index5))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Caption is not null)
                target.Values.Add("Caption", row.Caption);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Attribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Attribute, row.Attribute.Id, index2, "AttributeTranslation.Attribute");
                target.RelationshipIds.Add("AttributeId", row.Attribute.Id);
            }

            if (row.Culture is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Culture, row.Culture.Id, index8, "AttributeTranslation.Culture");
                target.RelationshipIds.Add("CultureId", row.Culture.Id);
            }

            instance.GetOrCreateEntityRecords("AttributeTranslation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index6))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.AggregateFunction is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.AggregateFunction, row.AggregateFunction.Id, index0, "AverageAggregateFunction.AggregateFunction");
                target.RelationshipIds.Add("AggregateFunctionId", row.AggregateFunction.Id);
            }

            instance.GetOrCreateEntityRecords("AverageAggregateFunction").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index7))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.AggregateFunction is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.AggregateFunction, row.AggregateFunction.Id, index0, "CountAggregateFunction.AggregateFunction");
                target.RelationshipIds.Add("AggregateFunctionId", row.AggregateFunction.Id);
            }

            instance.GetOrCreateEntityRecords("CountAggregateFunction").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index8))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.AnalyticsModel is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.AnalyticsModel, row.AnalyticsModel.Id, index1, "Culture.AnalyticsModel");
                target.RelationshipIds.Add("AnalyticsModelId", row.AnalyticsModel.Id);
            }

            instance.GetOrCreateEntityRecords("Culture").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index9))
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
            if (row.SourceKind is not null)
                target.Values.Add("SourceKind", row.SourceKind);
            if (row.AnalyticsModel is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.AnalyticsModel, row.AnalyticsModel.Id, index1, "DataSource.AnalyticsModel");
                target.RelationshipIds.Add("AnalyticsModelId", row.AnalyticsModel.Id);
            }

            instance.GetOrCreateEntityRecords("DataSource").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index10))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.AggregateFunction is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.AggregateFunction, row.AggregateFunction.Id, index0, "DistinctCountAggregateFunction.AggregateFunction");
                target.RelationshipIds.Add("AggregateFunctionId", row.AggregateFunction.Id);
            }

            instance.GetOrCreateEntityRecords("DistinctCountAggregateFunction").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index11))
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
            if (row.Kind is not null)
                target.Values.Add("Kind", row.Kind);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Table is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Table, row.Table.Id, index29, "Hierarchy.Table");
                target.RelationshipIds.Add("TableId", row.Table.Id);
            }

            instance.GetOrCreateEntityRecords("Hierarchy").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index12))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.Attribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Attribute, row.Attribute.Id, index2, "HierarchyLevel.Attribute");
                target.RelationshipIds.Add("AttributeId", row.Attribute.Id);
            }

            if (row.Hierarchy is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Hierarchy, row.Hierarchy.Id, index11, "HierarchyLevel.Hierarchy");
                target.RelationshipIds.Add("HierarchyId", row.Hierarchy.Id);
            }

            instance.GetOrCreateEntityRecords("HierarchyLevel").Add(target);
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
            if (row.Culture is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Culture, row.Culture.Id, index8, "HierarchyTranslation.Culture");
                target.RelationshipIds.Add("CultureId", row.Culture.Id);
            }

            if (row.Hierarchy is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Hierarchy, row.Hierarchy.Id, index11, "HierarchyTranslation.Hierarchy");
                target.RelationshipIds.Add("HierarchyId", row.Hierarchy.Id);
            }

            instance.GetOrCreateEntityRecords("HierarchyTranslation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index14))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.AggregateFunction is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.AggregateFunction, row.AggregateFunction.Id, index0, "MaximumAggregateFunction.AggregateFunction");
                target.RelationshipIds.Add("AggregateFunctionId", row.AggregateFunction.Id);
            }

            instance.GetOrCreateEntityRecords("MaximumAggregateFunction").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index15))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DataTypeId is not null)
                target.Values.Add("DataTypeId", row.DataTypeId);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.DisplayFolder is not null)
                target.Values.Add("DisplayFolder", row.DisplayFolder);
            if (row.FormatString is not null)
                target.Values.Add("FormatString", row.FormatString);
            if (row.IsHidden is not null)
                target.Values.Add("IsHidden", row.IsHidden);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.AggregateFunction is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.AggregateFunction, row.AggregateFunction.Id, index0, "Measure.AggregateFunction");
                target.RelationshipIds.Add("AggregateFunctionId", row.AggregateFunction.Id);
            }

            if (row.SourceAttribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SourceAttribute, row.SourceAttribute.Id, index2, "Measure.SourceAttribute");
                target.RelationshipIds.Add("SourceAttributeId", row.SourceAttribute.Id);
            }

            if (row.Table is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Table, row.Table.Id, index29, "Measure.Table");
                target.RelationshipIds.Add("TableId", row.Table.Id);
            }

            instance.GetOrCreateEntityRecords("Measure").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index16))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Caption is not null)
                target.Values.Add("Caption", row.Caption);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Culture is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Culture, row.Culture.Id, index8, "MeasureTranslation.Culture");
                target.RelationshipIds.Add("CultureId", row.Culture.Id);
            }

            if (row.Measure is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Measure, row.Measure.Id, index15, "MeasureTranslation.Measure");
                target.RelationshipIds.Add("MeasureId", row.Measure.Id);
            }

            instance.GetOrCreateEntityRecords("MeasureTranslation").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index17))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.AggregateFunction is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.AggregateFunction, row.AggregateFunction.Id, index0, "MinimumAggregateFunction.AggregateFunction");
                target.RelationshipIds.Add("AggregateFunctionId", row.AggregateFunction.Id);
            }

            instance.GetOrCreateEntityRecords("MinimumAggregateFunction").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index18))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.AnalyticsModel is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.AnalyticsModel, row.AnalyticsModel.Id, index1, "Perspective.AnalyticsModel");
                target.RelationshipIds.Add("AnalyticsModelId", row.AnalyticsModel.Id);
            }

            instance.GetOrCreateEntityRecords("Perspective").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index19))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Attribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Attribute, row.Attribute.Id, index2, "PerspectiveAttribute.Attribute");
                target.RelationshipIds.Add("AttributeId", row.Attribute.Id);
            }

            if (row.Perspective is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Perspective, row.Perspective.Id, index18, "PerspectiveAttribute.Perspective");
                target.RelationshipIds.Add("PerspectiveId", row.Perspective.Id);
            }

            instance.GetOrCreateEntityRecords("PerspectiveAttribute").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index20))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Hierarchy is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Hierarchy, row.Hierarchy.Id, index11, "PerspectiveHierarchy.Hierarchy");
                target.RelationshipIds.Add("HierarchyId", row.Hierarchy.Id);
            }

            if (row.Perspective is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Perspective, row.Perspective.Id, index18, "PerspectiveHierarchy.Perspective");
                target.RelationshipIds.Add("PerspectiveId", row.Perspective.Id);
            }

            instance.GetOrCreateEntityRecords("PerspectiveHierarchy").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index21))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Measure is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Measure, row.Measure.Id, index15, "PerspectiveMeasure.Measure");
                target.RelationshipIds.Add("MeasureId", row.Measure.Id);
            }

            if (row.Perspective is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Perspective, row.Perspective.Id, index18, "PerspectiveMeasure.Perspective");
                target.RelationshipIds.Add("PerspectiveId", row.Perspective.Id);
            }

            instance.GetOrCreateEntityRecords("PerspectiveMeasure").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index22))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Perspective is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Perspective, row.Perspective.Id, index18, "PerspectiveTable.Perspective");
                target.RelationshipIds.Add("PerspectiveId", row.Perspective.Id);
            }

            if (row.Table is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Table, row.Table.Id, index29, "PerspectiveTable.Table");
                target.RelationshipIds.Add("TableId", row.Table.Id);
            }

            instance.GetOrCreateEntityRecords("PerspectiveTable").Add(target);
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
            if (row.Culture is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Culture, row.Culture.Id, index8, "PerspectiveTranslation.Culture");
                target.RelationshipIds.Add("CultureId", row.Culture.Id);
            }

            if (row.Perspective is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Perspective, row.Perspective.Id, index18, "PerspectiveTranslation.Perspective");
                target.RelationshipIds.Add("PerspectiveId", row.Perspective.Id);
            }

            instance.GetOrCreateEntityRecords("PerspectiveTranslation").Add(target);
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
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.IsActive is not null)
                target.Values.Add("IsActive", row.IsActive);
            if (row.IsRequired is not null)
                target.Values.Add("IsRequired", row.IsRequired);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.RelationshipKind is not null)
                target.Values.Add("RelationshipKind", row.RelationshipKind);
            if (row.RoleName is not null)
                target.Values.Add("RoleName", row.RoleName);
            if (row.FromAttribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.FromAttribute, row.FromAttribute.Id, index2, "Relationship.FromAttribute");
                target.RelationshipIds.Add("FromAttributeId", row.FromAttribute.Id);
            }

            if (row.FromTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.FromTable, row.FromTable.Id, index29, "Relationship.FromTable");
                target.RelationshipIds.Add("FromTableId", row.FromTable.Id);
            }

            if (row.GranularityAttribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.GranularityAttribute, row.GranularityAttribute.Id, index2, "Relationship.GranularityAttribute");
                target.RelationshipIds.Add("GranularityAttributeId", row.GranularityAttribute.Id);
            }

            if (row.IntermediateTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.IntermediateTable, row.IntermediateTable.Id, index29, "Relationship.IntermediateTable");
                target.RelationshipIds.Add("IntermediateTableId", row.IntermediateTable.Id);
            }

            if (row.ToAttribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ToAttribute, row.ToAttribute.Id, index2, "Relationship.ToAttribute");
                target.RelationshipIds.Add("ToAttributeId", row.ToAttribute.Id);
            }

            if (row.ToTable is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.ToTable, row.ToTable.Id, index29, "Relationship.ToTable");
                target.RelationshipIds.Add("ToTableId", row.ToTable.Id);
            }

            instance.GetOrCreateEntityRecords("Relationship").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index25))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MemberKind is not null)
                target.Values.Add("MemberKind", row.MemberKind);
            if (row.MemberName is not null)
                target.Values.Add("MemberName", row.MemberName);
            if (row.SecurityRole is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SecurityRole, row.SecurityRole.Id, index26, "RoleMember.SecurityRole");
                target.RelationshipIds.Add("SecurityRoleId", row.SecurityRole.Id);
            }

            instance.GetOrCreateEntityRecords("RoleMember").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index26))
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
            if (row.AnalyticsModel is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.AnalyticsModel, row.AnalyticsModel.Id, index1, "SecurityRole.AnalyticsModel");
                target.RelationshipIds.Add("AnalyticsModelId", row.AnalyticsModel.Id);
            }

            instance.GetOrCreateEntityRecords("SecurityRole").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index27))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.SortAttribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SortAttribute, row.SortAttribute.Id, index2, "SortByAttribute.SortAttribute");
                target.RelationshipIds.Add("SortAttributeId", row.SortAttribute.Id);
            }

            if (row.SourceAttribute is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SourceAttribute, row.SourceAttribute.Id, index2, "SortByAttribute.SourceAttribute");
                target.RelationshipIds.Add("SourceAttributeId", row.SourceAttribute.Id);
            }

            instance.GetOrCreateEntityRecords("SortByAttribute").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index28))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.AggregateFunction is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.AggregateFunction, row.AggregateFunction.Id, index0, "SumAggregateFunction.AggregateFunction");
                target.RelationshipIds.Add("AggregateFunctionId", row.AggregateFunction.Id);
            }

            instance.GetOrCreateEntityRecords("SumAggregateFunction").Add(target);
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
            if (row.DisplayFolder is not null)
                target.Values.Add("DisplayFolder", row.DisplayFolder);
            if (row.IsHidden is not null)
                target.Values.Add("IsHidden", row.IsHidden);
            if (row.Kind is not null)
                target.Values.Add("Kind", row.Kind);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.AnalyticsModel is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.AnalyticsModel, row.AnalyticsModel.Id, index1, "Table.AnalyticsModel");
                target.RelationshipIds.Add("AnalyticsModelId", row.AnalyticsModel.Id);
            }

            instance.GetOrCreateEntityRecords("Table").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index30))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.MetadataPermission is not null)
                target.Values.Add("MetadataPermission", row.MetadataPermission);
            if (row.SecurityRole is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.SecurityRole, row.SecurityRole.Id, index26, "TablePermission.SecurityRole");
                target.RelationshipIds.Add("SecurityRoleId", row.SecurityRole.Id);
            }

            if (row.Table is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Table, row.Table.Id, index29, "TablePermission.Table");
                target.RelationshipIds.Add("TableId", row.Table.Id);
            }

            instance.GetOrCreateEntityRecords("TablePermission").Add(target);
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
            if (row.Culture is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Culture, row.Culture.Id, index8, "TableTranslation.Culture");
                target.RelationshipIds.Add("CultureId", row.Culture.Id);
            }

            if (row.Table is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Table, row.Table.Id, index29, "TableTranslation.Table");
                target.RelationshipIds.Add("TableId", row.Table.Id);
            }

            instance.GetOrCreateEntityRecords("TableTranslation").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}