// Derived from the modeled contract. Keep outside authored workspace source sets.
#nullable enable
namespace MetaOrchestration;
public sealed class MetaOrchestrationWorkspaceBinding : global::Meta.TypedModels.ITypedWorkspaceBinding<global::MetaOrchestration.MetaOrchestrationModel>
{
    private const string ContractSignature = "6d6f64656c7c4d6574614f726368657374726174696f6e0a656e746974797c446174614f626a6563747c446174614f626a6563744c6973740a70726f70657274797c446174614f626a6563747c4e6f726d616c697a65644b65797c72657175697265640a70726f70657274797c446174614f626a6563747c53716c4964656e7469666965727c72657175697265640a72656c6174696f6e736869707c446174614f626a6563747c4f726368657374726174696f6e506c616e7c4f726368657374726174696f6e506c616e49647c72657175697265640a656e746974797c446570656e64656e637949737375657c446570656e64656e637949737375654c6973740a70726f70657274797c446570656e64656e637949737375657c426c6f636b734175746f6d6174696352756e506c616e6e696e677c72657175697265640a70726f70657274797c446570656e64656e637949737375657c426c6f636b734461677c72657175697265640a70726f70657274797c446570656e64656e637949737375657c436f64657c72657175697265640a70726f70657274797c446570656e64656e637949737375657c4973737565446f6d61696e7c72657175697265640a70726f70657274797c446570656e64656e637949737375657c4d6573736167657c72657175697265640a70726f70657274797c446570656e64656e637949737375657c53657665726974797c72657175697265640a72656c6174696f6e736869707c446570656e64656e637949737375657c446174614f626a6563747c446174614f626a65637449647c6e756c6c61626c650a72656c6174696f6e736869707c446570656e64656e637949737375657c4f726368657374726174696f6e506c616e7c4f726368657374726174696f6e506c616e49647c72657175697265640a656e746974797c446570656e64656e63794973737565506970656c696e657c446570656e64656e63794973737565506970656c696e654c6973740a70726f70657274797c446570656e64656e63794973737565506970656c696e657c526f6c657c72657175697265640a72656c6174696f6e736869707c446570656e64656e63794973737565506970656c696e657c446570656e64656e637949737375657c446570656e64656e6379497373756549647c72657175697265640a72656c6174696f6e736869707c446570656e64656e63794973737565506970656c696e657c506970656c696e655265666572656e63657c506970656c696e655265666572656e636549647c72657175697265640a656e746974797c4c6f636b436f6d7061746962696c697479506f6c6963797c4c6f636b436f6d7061746962696c697479506f6c6963794c6973740a70726f70657274797c4c6f636b436f6d7061746962696c697479506f6c6963797c4c6566744566666563747c72657175697265640a70726f70657274797c4c6f636b436f6d7061746962696c697479506f6c6963797c4c6f636b4265686176696f727c72657175697265640a70726f70657274797c4c6f636b436f6d7061746962696c697479506f6c6963797c506f6c6963794b696e647c72657175697265640a70726f70657274797c4c6f636b436f6d7061746962696c697479506f6c6963797c526561736f6e7c6e756c6c61626c650a70726f70657274797c4c6f636b436f6d7061746962696c697479506f6c6963797c52696768744566666563747c72657175697265640a70726f70657274797c4c6f636b436f6d7061746962696c697479506f6c6963797c5374617475737c72657175697265640a72656c6174696f6e736869707c4c6f636b436f6d7061746962696c697479506f6c6963797c446174614f626a6563747c446174614f626a65637449647c72657175697265640a72656c6174696f6e736869707c4c6f636b436f6d7061746962696c697479506f6c6963797c446570656e64656e637949737375657c446570656e64656e6379497373756549647c6e756c6c61626c650a72656c6174696f6e736869707c4c6f636b436f6d7061746962696c697479506f6c6963797c4f726368657374726174696f6e506c616e7c4f726368657374726174696f6e506c616e49647c72657175697265640a656e746974797c4f626a6563744163636573737c4f626a6563744163636573734c6973740a70726f70657274797c4f626a6563744163636573737c4163636573734b696e647c72657175697265640a70726f70657274797c4f626a6563744163636573737c416363657373526f6c657c72657175697265640a70726f70657274797c4f626a6563744163636573737c4f7065726174696f6e4b696e647c6e756c6c61626c650a70726f70657274797c4f626a6563744163636573737c4f7264696e616c7c72657175697265640a70726f70657274797c4f626a6563744163636573737c526561736f6e7c6e756c6c61626c650a72656c6174696f6e736869707c4f626a6563744163636573737c446174614f626a6563747c446174614f626a65637449647c72657175697265640a72656c6174696f6e736869707c4f626a6563744163636573737c5461736b41636365737350726f66696c657c5461736b41636365737350726f66696c6549647c72657175697265640a656e746974797c4f726368657374726174696f6e506c616e7c4f726368657374726174696f6e506c616e4c6973740a70726f70657274797c4f726368657374726174696f6e506c616e7c4461675374617475737c72657175697265640a70726f70657274797c4f726368657374726174696f6e506c616e7c4465736372697074696f6e7c6e756c6c61626c650a70726f70657274797c4f726368657374726174696f6e506c616e7c44657465726d696e69736d5374617475737c72657175697265640a70726f70657274797c4f726368657374726174696f6e506c616e7c4e616d657c72657175697265640a70726f70657274797c4f726368657374726174696f6e506c616e7c53796e6368726f6e697a6174696f6e5374617475737c72657175697265640a656e746974797c506970656c696e65446570656e64656e63797c506970656c696e65446570656e64656e63794c6973740a70726f70657274797c506970656c696e65446570656e64656e63797c446570656e64656e63794b696e647c72657175697265640a70726f70657274797c506970656c696e65446570656e64656e63797c526561736f6e7c6e756c6c61626c650a72656c6174696f6e736869707c506970656c696e65446570656e64656e63797c4f726368657374726174696f6e506c616e7c4f726368657374726174696f6e506c616e49647c72657175697265640a72656c6174696f6e736869707c506970656c696e65446570656e64656e63797c506970656c696e655265666572656e63657c5072656465636573736f7249647c72657175697265640a72656c6174696f6e736869707c506970656c696e65446570656e64656e63797c506970656c696e655265666572656e63657c537563636573736f7249647c72657175697265640a656e746974797c506970656c696e654f626a6563744163636573737c506970656c696e654f626a6563744163636573734c6973740a70726f70657274797c506970656c696e654f626a6563744163636573737c4163636573734b696e647c72657175697265640a70726f70657274797c506970656c696e654f626a6563744163636573737c526561736f6e7c6e756c6c61626c650a72656c6174696f6e736869707c506970656c696e654f626a6563744163636573737c446174614f626a6563747c446174614f626a65637449647c72657175697265640a72656c6174696f6e736869707c506970656c696e654f626a6563744163636573737c506970656c696e655265666572656e63657c506970656c696e655265666572656e636549647c72657175697265640a656e746974797c506970656c696e655265666572656e63657c506970656c696e655265666572656e63654c6973740a70726f70657274797c506970656c696e655265666572656e63657c4d657461506970656c696e65506970656c696e6549647c72657175697265640a70726f70657274797c506970656c696e655265666572656e63657c4e616d657c72657175697265640a70726f70657274797c506970656c696e655265666572656e63657c506970656c696e65576f726b7370616365506174687c6e756c6c61626c650a72656c6174696f6e736869707c506970656c696e655265666572656e63657c4f726368657374726174696f6e506c616e7c4f726368657374726174696f6e506c616e49647c72657175697265640a656e746974797c506c616e6e65645461736b7c506c616e6e65645461736b4c6973740a70726f70657274797c506c616e6e65645461736b7c4f7264696e616c7c72657175697265640a70726f70657274797c506c616e6e65645461736b7c526561736f6e7c6e756c6c61626c650a72656c6174696f6e736869707c506c616e6e65645461736b7c506970656c696e655265666572656e63657c506970656c696e655265666572656e636549647c72657175697265640a72656c6174696f6e736869707c506c616e6e65645461736b7c52756e506c616e7c52756e506c616e49647c72657175697265640a72656c6174696f6e736869707c506c616e6e65645461736b7c5461736b41636365737350726f66696c657c5461736b41636365737350726f66696c6549647c72657175697265640a656e746974797c506c616e6e65645461736b4c6f636b7c506c616e6e65645461736b4c6f636b4c6973740a70726f70657274797c506c616e6e65645461736b4c6f636b7c4c6f636b4d6f64657c72657175697265640a70726f70657274797c506c616e6e65645461736b4c6f636b7c526561736f6e7c6e756c6c61626c650a72656c6174696f6e736869707c506c616e6e65645461736b4c6f636b7c446174614f626a6563747c446174614f626a65637449647c72657175697265640a72656c6174696f6e736869707c506c616e6e65645461736b4c6f636b7c4c6f636b436f6d7061746962696c697479506f6c6963797c4c6f636b436f6d7061746962696c697479506f6c69637949647c6e756c6c61626c650a72656c6174696f6e736869707c506c616e6e65645461736b4c6f636b7c506c616e6e65645461736b7c506c616e6e65645461736b49647c72657175697265640a72656c6174696f6e736869707c506c616e6e65645461736b4c6f636b7c5461736b4f626a6563744566666563747c5461736b4f626a65637445666665637449647c72657175697265640a656e746974797c5265747279506f6c6963797c5265747279506f6c6963794c6973740a70726f70657274797c5265747279506f6c6963797c4261636b6f66664d756c7469706c6965727c72657175697265640a70726f70657274797c5265747279506f6c6963797c496e697469616c44656c61794d696c6c697365636f6e64737c72657175697265640a70726f70657274797c5265747279506f6c6963797c4d6178417474656d7074737c72657175697265640a70726f70657274797c5265747279506f6c6963797c4d617844656c61794d696c6c697365636f6e64737c72657175697265640a70726f70657274797c5265747279506f6c6963797c4e616d657c72657175697265640a70726f70657274797c5265747279506f6c6963797c506f6c6963794b696e647c72657175697265640a70726f70657274797c5265747279506f6c6963797c526561736f6e7c6e756c6c61626c650a70726f70657274797c5265747279506f6c6963797c5265747279526561644f6e6c795461736b73427944656661756c747c72657175697265640a70726f70657274797c5265747279506f6c6963797c526574727957726974655461736b73427944656661756c747c72657175697265640a70726f70657274797c5265747279506f6c6963797c5374617475737c72657175697265640a72656c6174696f6e736869707c5265747279506f6c6963797c4f726368657374726174696f6e506c616e7c4f726368657374726174696f6e506c616e49647c72657175697265640a656e746974797c5265747279506f6c6963794661696c757265436c6173737c5265747279506f6c6963794661696c757265436c6173734c6973740a70726f70657274797c5265747279506f6c6963794661696c757265436c6173737c4661696c757265436c6173737c72657175697265640a70726f70657274797c5265747279506f6c6963794661696c757265436c6173737c526561736f6e7c6e756c6c61626c650a70726f70657274797c5265747279506f6c6963794661696c757265436c6173737c52657472794265686176696f727c72657175697265640a72656c6174696f6e736869707c5265747279506f6c6963794661696c757265436c6173737c5265747279506f6c6963797c5265747279506f6c69637949647c72657175697265640a656e746974797c52756e506c616e7c52756e506c616e4c6973740a70726f70657274797c52756e506c616e7c4e616d657c72657175697265640a70726f70657274797c52756e506c616e7c526561736f6e7c6e756c6c61626c650a70726f70657274797c52756e506c616e7c52756e506c616e5374617475737c72657175697265640a72656c6174696f6e736869707c52756e506c616e7c4f726368657374726174696f6e506c616e7c4f726368657374726174696f6e506c616e49647c72657175697265640a656e746974797c52756e506c616e5265747279506f6c6963797c52756e506c616e5265747279506f6c6963794c6973740a70726f70657274797c52756e506c616e5265747279506f6c6963797c506f6c696379526f6c657c72657175697265640a70726f70657274797c52756e506c616e5265747279506f6c6963797c526561736f6e7c6e756c6c61626c650a72656c6174696f6e736869707c52756e506c616e5265747279506f6c6963797c5265747279506f6c6963797c5265747279506f6c69637949647c72657175697265640a72656c6174696f6e736869707c52756e506c616e5265747279506f6c6963797c52756e506c616e7c52756e506c616e49647c72657175697265640a656e746974797c5461736b41636365737350726f66696c657c5461736b41636365737350726f66696c654c6973740a70726f70657274797c5461736b41636365737350726f66696c657c42696e64696e67576f726b7370616365506174687c6e756c6c61626c650a70726f70657274797c5461736b41636365737350726f66696c657c4d657461506970656c696e65506970656c696e655461736b49647c72657175697265640a70726f70657274797c5461736b41636365737350726f66696c657c4f7264696e616c7c72657175697265640a70726f70657274797c5461736b41636365737350726f66696c657c53746174656d656e744b696e647c72657175697265640a70726f70657274797c5461736b41636365737350726f66696c657c5461736b4b696e647c72657175697265640a70726f70657274797c5461736b41636365737350726f66696c657c5461736b4e616d657c72657175697265640a70726f70657274797c5461736b41636365737350726f66696c657c5472616e73666f726d42696e64696e6749647c6e756c6c61626c650a70726f70657274797c5461736b41636365737350726f66696c657c5472616e73666f726d53637269707449647c6e756c6c61626c650a70726f70657274797c5461736b41636365737350726f66696c657c5472616e73666f726d5363726970744e616d657c6e756c6c61626c650a70726f70657274797c5461736b41636365737350726f66696c657c5472616e73666f726d576f726b7370616365506174687c6e756c6c61626c650a72656c6174696f6e736869707c5461736b41636365737350726f66696c657c506970656c696e655265666572656e63657c506970656c696e655265666572656e636549647c72657175697265640a656e746974797c5461736b446570656e64656e63797c5461736b446570656e64656e63794c6973740a70726f70657274797c5461736b446570656e64656e63797c446570656e64656e6379436f6e646974696f6e7c72657175697265640a70726f70657274797c5461736b446570656e64656e63797c446570656e64656e63794b696e647c72657175697265640a70726f70657274797c5461736b446570656e64656e63797c526561736f6e7c6e756c6c61626c650a72656c6174696f6e736869707c5461736b446570656e64656e63797c446174614f626a6563747c446174614f626a65637449647c6e756c6c61626c650a72656c6174696f6e736869707c5461736b446570656e64656e63797c4f726368657374726174696f6e506c616e7c4f726368657374726174696f6e506c616e49647c72657175697265640a72656c6174696f6e736869707c5461736b446570656e64656e63797c5461736b41636365737350726f66696c657c5072656465636573736f7249647c72657175697265640a72656c6174696f6e736869707c5461736b446570656e64656e63797c5461736b41636365737350726f66696c657c537563636573736f7249647c72657175697265640a656e746974797c5461736b4f626a6563744566666563747c5461736b4f626a6563744566666563744c6973740a70726f70657274797c5461736b4f626a6563744566666563747c416363657373446972656374696f6e7c72657175697265640a70726f70657274797c5461736b4f626a6563744566666563747c416363657373507572706f73657c72657175697265640a70726f70657274797c5461736b4f626a6563744566666563747c4372656174657344617461446570656e64656e63797c72657175697265640a70726f70657274797c5461736b4f626a6563744566666563747c49735075626c697368656450726f64756365727c72657175697265640a70726f70657274797c5461736b4f626a6563744566666563747c4c6f636b4d6f64657c72657175697265640a70726f70657274797c5461736b4f626a6563744566666563747c526561736f6e7c6e756c6c61626c650a70726f70657274797c5461736b4f626a6563744566666563747c526571756972657353796e6368726f6e697a6174696f6e7c72657175697265640a70726f70657274797c5461736b4f626a6563744566666563747c57726974654566666563747c72657175697265640a72656c6174696f6e736869707c5461736b4f626a6563744566666563747c446174614f626a6563747c446174614f626a65637449647c72657175697265640a72656c6174696f6e736869707c5461736b4f626a6563744566666563747c5461736b41636365737350726f66696c657c5461736b41636365737350726f66696c6549647c72657175697265640a656e746974797c5461736b4f72646572696e675265736f6c7574696f6e7c5461736b4f72646572696e675265736f6c7574696f6e4c6973740a70726f70657274797c5461736b4f72646572696e675265736f6c7574696f6e7c446570656e64656e6379436f6e646974696f6e7c72657175697265640a70726f70657274797c5461736b4f72646572696e675265736f6c7574696f6e7c4e616d657c72657175697265640a70726f70657274797c5461736b4f72646572696e675265736f6c7574696f6e7c526561736f6e7c6e756c6c61626c650a70726f70657274797c5461736b4f72646572696e675265736f6c7574696f6e7c5265736f6c7574696f6e4b696e647c72657175697265640a70726f70657274797c5461736b4f72646572696e675265736f6c7574696f6e7c5374617475737c72657175697265640a72656c6174696f6e736869707c5461736b4f72646572696e675265736f6c7574696f6e7c446174614f626a6563747c446174614f626a65637449647c6e756c6c61626c650a72656c6174696f6e736869707c5461736b4f72646572696e675265736f6c7574696f6e7c446570656e64656e637949737375657c446570656e64656e6379497373756549647c6e756c6c61626c650a72656c6174696f6e736869707c5461736b4f72646572696e675265736f6c7574696f6e7c4f726368657374726174696f6e506c616e7c4f726368657374726174696f6e506c616e49647c72657175697265640a72656c6174696f6e736869707c5461736b4f72646572696e675265736f6c7574696f6e7c5461736b41636365737350726f66696c657c5072656465636573736f7249647c72657175697265640a72656c6174696f6e736869707c5461736b4f72646572696e675265736f6c7574696f6e7c5461736b41636365737350726f66696c657c537563636573736f7249647c7265717569726564";
    public global::MetaOrchestration.MetaOrchestrationModel Read(global::Meta.Operations.Domain.InMemoryWorkspace state)
    {
        global::System.ArgumentNullException.ThrowIfNull(state);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(state.Model, ContractSignature);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        var typed = new global::MetaOrchestration.MetaOrchestrationModel();
        var index0 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.DataObject>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DataObject"))
        {
            var row = new global::MetaOrchestration.DataObject
            {
                Id = source.Id
            };
            row.NormalizedKey = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "NormalizedKey");
            row.SqlIdentifier = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SqlIdentifier");
            typed.DataObjectList.Add(row);
            index0.Add(source.Id, row);
        }

        var index1 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.DependencyIssue>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DependencyIssue"))
        {
            var row = new global::MetaOrchestration.DependencyIssue
            {
                Id = source.Id
            };
            row.BlocksAutomaticRunPlanning = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "BlocksAutomaticRunPlanning");
            row.BlocksDag = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "BlocksDag");
            row.Code = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Code");
            row.IssueDomain = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "IssueDomain");
            row.Message = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Message");
            row.Severity = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Severity");
            typed.DependencyIssueList.Add(row);
            index1.Add(source.Id, row);
        }

        var index2 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.DependencyIssuePipeline>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DependencyIssuePipeline"))
        {
            var row = new global::MetaOrchestration.DependencyIssuePipeline
            {
                Id = source.Id
            };
            row.Role = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Role");
            typed.DependencyIssuePipelineList.Add(row);
            index2.Add(source.Id, row);
        }

        var index3 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.LockCompatibilityPolicy>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "LockCompatibilityPolicy"))
        {
            var row = new global::MetaOrchestration.LockCompatibilityPolicy
            {
                Id = source.Id
            };
            row.LeftEffect = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LeftEffect");
            row.LockBehavior = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LockBehavior");
            row.PolicyKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "PolicyKind");
            row.Reason = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Reason");
            row.RightEffect = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "RightEffect");
            row.Status = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Status");
            typed.LockCompatibilityPolicyList.Add(row);
            index3.Add(source.Id, row);
        }

        var index4 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.ObjectAccess>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ObjectAccess"))
        {
            var row = new global::MetaOrchestration.ObjectAccess
            {
                Id = source.Id
            };
            row.AccessKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "AccessKind");
            row.AccessRole = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "AccessRole");
            row.OperationKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "OperationKind");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            row.Reason = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Reason");
            typed.ObjectAccessList.Add(row);
            index4.Add(source.Id, row);
        }

        var index5 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.OrchestrationPlan>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "OrchestrationPlan"))
        {
            var row = new global::MetaOrchestration.OrchestrationPlan
            {
                Id = source.Id
            };
            row.DagStatus = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DagStatus");
            row.Description = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Description");
            row.DeterminismStatus = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DeterminismStatus");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.SynchronizationStatus = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "SynchronizationStatus");
            typed.OrchestrationPlanList.Add(row);
            index5.Add(source.Id, row);
        }

        var index6 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.PipelineDependency>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PipelineDependency"))
        {
            var row = new global::MetaOrchestration.PipelineDependency
            {
                Id = source.Id
            };
            row.DependencyKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DependencyKind");
            row.Reason = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Reason");
            typed.PipelineDependencyList.Add(row);
            index6.Add(source.Id, row);
        }

        var index7 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.PipelineObjectAccess>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PipelineObjectAccess"))
        {
            var row = new global::MetaOrchestration.PipelineObjectAccess
            {
                Id = source.Id
            };
            row.AccessKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "AccessKind");
            row.Reason = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Reason");
            typed.PipelineObjectAccessList.Add(row);
            index7.Add(source.Id, row);
        }

        var index8 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.PipelineReference>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PipelineReference"))
        {
            var row = new global::MetaOrchestration.PipelineReference
            {
                Id = source.Id
            };
            row.MetaPipelinePipelineId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaPipelinePipelineId");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.PipelineWorkspacePath = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "PipelineWorkspacePath");
            typed.PipelineReferenceList.Add(row);
            index8.Add(source.Id, row);
        }

        var index9 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.PlannedTask>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PlannedTask"))
        {
            var row = new global::MetaOrchestration.PlannedTask
            {
                Id = source.Id
            };
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            row.Reason = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Reason");
            typed.PlannedTaskList.Add(row);
            index9.Add(source.Id, row);
        }

        var index10 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.PlannedTaskLock>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PlannedTaskLock"))
        {
            var row = new global::MetaOrchestration.PlannedTaskLock
            {
                Id = source.Id
            };
            row.LockMode = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LockMode");
            row.Reason = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Reason");
            typed.PlannedTaskLockList.Add(row);
            index10.Add(source.Id, row);
        }

        var index11 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.RetryPolicy>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RetryPolicy"))
        {
            var row = new global::MetaOrchestration.RetryPolicy
            {
                Id = source.Id
            };
            row.BackoffMultiplier = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "BackoffMultiplier");
            row.InitialDelayMilliseconds = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "InitialDelayMilliseconds");
            row.MaxAttempts = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MaxAttempts");
            row.MaxDelayMilliseconds = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MaxDelayMilliseconds");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.PolicyKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "PolicyKind");
            row.Reason = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Reason");
            row.RetryReadOnlyTasksByDefault = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "RetryReadOnlyTasksByDefault");
            row.RetryWriteTasksByDefault = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "RetryWriteTasksByDefault");
            row.Status = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Status");
            typed.RetryPolicyList.Add(row);
            index11.Add(source.Id, row);
        }

        var index12 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.RetryPolicyFailureClass>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RetryPolicyFailureClass"))
        {
            var row = new global::MetaOrchestration.RetryPolicyFailureClass
            {
                Id = source.Id
            };
            row.FailureClass = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "FailureClass");
            row.Reason = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Reason");
            row.RetryBehavior = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "RetryBehavior");
            typed.RetryPolicyFailureClassList.Add(row);
            index12.Add(source.Id, row);
        }

        var index13 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.RunPlan>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RunPlan"))
        {
            var row = new global::MetaOrchestration.RunPlan
            {
                Id = source.Id
            };
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Reason = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Reason");
            row.RunPlanStatus = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "RunPlanStatus");
            typed.RunPlanList.Add(row);
            index13.Add(source.Id, row);
        }

        var index14 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.RunPlanRetryPolicy>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RunPlanRetryPolicy"))
        {
            var row = new global::MetaOrchestration.RunPlanRetryPolicy
            {
                Id = source.Id
            };
            row.PolicyRole = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "PolicyRole");
            row.Reason = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Reason");
            typed.RunPlanRetryPolicyList.Add(row);
            index14.Add(source.Id, row);
        }

        var index15 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.TaskAccessProfile>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TaskAccessProfile"))
        {
            var row = new global::MetaOrchestration.TaskAccessProfile
            {
                Id = source.Id
            };
            row.BindingWorkspacePath = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "BindingWorkspacePath");
            row.MetaPipelinePipelineTaskId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "MetaPipelinePipelineTaskId");
            row.Ordinal = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Ordinal");
            row.StatementKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "StatementKind");
            row.TaskKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "TaskKind");
            row.TaskName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "TaskName");
            row.TransformBindingId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "TransformBindingId");
            row.TransformScriptId = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "TransformScriptId");
            row.TransformScriptName = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "TransformScriptName");
            row.TransformWorkspacePath = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "TransformWorkspacePath");
            typed.TaskAccessProfileList.Add(row);
            index15.Add(source.Id, row);
        }

        var index16 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.TaskDependency>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TaskDependency"))
        {
            var row = new global::MetaOrchestration.TaskDependency
            {
                Id = source.Id
            };
            row.DependencyCondition = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DependencyCondition");
            row.DependencyKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DependencyKind");
            row.Reason = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Reason");
            typed.TaskDependencyList.Add(row);
            index16.Add(source.Id, row);
        }

        var index17 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.TaskObjectEffect>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TaskObjectEffect"))
        {
            var row = new global::MetaOrchestration.TaskObjectEffect
            {
                Id = source.Id
            };
            row.AccessDirection = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "AccessDirection");
            row.AccessPurpose = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "AccessPurpose");
            row.CreatesDataDependency = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "CreatesDataDependency");
            row.IsPublishedProducer = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "IsPublishedProducer");
            row.LockMode = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "LockMode");
            row.Reason = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Reason");
            row.RequiresSynchronization = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "RequiresSynchronization");
            row.WriteEffect = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "WriteEffect");
            typed.TaskObjectEffectList.Add(row);
            index17.Add(source.Id, row);
        }

        var index18 = new global::System.Collections.Generic.Dictionary<string, global::MetaOrchestration.TaskOrderingResolution>(global::Meta.Operations.Domain.MetaIdentity.Comparer);
        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TaskOrderingResolution"))
        {
            var row = new global::MetaOrchestration.TaskOrderingResolution
            {
                Id = source.Id
            };
            row.DependencyCondition = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "DependencyCondition");
            row.Name = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Name");
            row.Reason = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalValue(source, "Reason");
            row.ResolutionKind = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "ResolutionKind");
            row.Status = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredValue(source, "Status");
            typed.TaskOrderingResolutionList.Add(row);
            index18.Add(source.Id, row);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DataObject"))
        {
            var row = index0[source.Id];
            row.OrchestrationPlan = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "OrchestrationPlanId", index5);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DependencyIssue"))
        {
            var row = index1[source.Id];
            row.DataObject = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalTarget(source, "DataObjectId", index0);
            row.OrchestrationPlan = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "OrchestrationPlanId", index5);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "DependencyIssuePipeline"))
        {
            var row = index2[source.Id];
            row.DependencyIssue = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DependencyIssueId", index1);
            row.PipelineReference = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PipelineReferenceId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "LockCompatibilityPolicy"))
        {
            var row = index3[source.Id];
            row.DataObject = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DataObjectId", index0);
            row.DependencyIssue = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalTarget(source, "DependencyIssueId", index1);
            row.OrchestrationPlan = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "OrchestrationPlanId", index5);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "ObjectAccess"))
        {
            var row = index4[source.Id];
            row.DataObject = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DataObjectId", index0);
            row.TaskAccessProfile = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TaskAccessProfileId", index15);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PipelineDependency"))
        {
            var row = index6[source.Id];
            row.OrchestrationPlan = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "OrchestrationPlanId", index5);
            row.Predecessor = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PredecessorId", index8);
            row.Successor = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SuccessorId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PipelineObjectAccess"))
        {
            var row = index7[source.Id];
            row.DataObject = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DataObjectId", index0);
            row.PipelineReference = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PipelineReferenceId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PipelineReference"))
        {
            var row = index8[source.Id];
            row.OrchestrationPlan = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "OrchestrationPlanId", index5);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PlannedTask"))
        {
            var row = index9[source.Id];
            row.PipelineReference = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PipelineReferenceId", index8);
            row.RunPlan = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RunPlanId", index13);
            row.TaskAccessProfile = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TaskAccessProfileId", index15);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "PlannedTaskLock"))
        {
            var row = index10[source.Id];
            row.DataObject = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DataObjectId", index0);
            row.LockCompatibilityPolicy = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalTarget(source, "LockCompatibilityPolicyId", index3);
            row.PlannedTask = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PlannedTaskId", index9);
            row.TaskObjectEffect = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TaskObjectEffectId", index17);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RetryPolicy"))
        {
            var row = index11[source.Id];
            row.OrchestrationPlan = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "OrchestrationPlanId", index5);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RetryPolicyFailureClass"))
        {
            var row = index12[source.Id];
            row.RetryPolicy = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RetryPolicyId", index11);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RunPlan"))
        {
            var row = index13[source.Id];
            row.OrchestrationPlan = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "OrchestrationPlanId", index5);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "RunPlanRetryPolicy"))
        {
            var row = index14[source.Id];
            row.RetryPolicy = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RetryPolicyId", index11);
            row.RunPlan = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "RunPlanId", index13);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TaskAccessProfile"))
        {
            var row = index15[source.Id];
            row.PipelineReference = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PipelineReferenceId", index8);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TaskDependency"))
        {
            var row = index16[source.Id];
            row.DataObject = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalTarget(source, "DataObjectId", index0);
            row.OrchestrationPlan = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "OrchestrationPlanId", index5);
            row.Predecessor = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PredecessorId", index15);
            row.Successor = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SuccessorId", index15);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TaskObjectEffect"))
        {
            var row = index17[source.Id];
            row.DataObject = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "DataObjectId", index0);
            row.TaskAccessProfile = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "TaskAccessProfileId", index15);
        }

        foreach (var source in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Records(state, "TaskOrderingResolution"))
        {
            var row = index18[source.Id];
            row.DataObject = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalTarget(source, "DataObjectId", index0);
            row.DependencyIssue = global::Meta.TypedModels.TypedWorkspaceBindingValidation.OptionalTarget(source, "DependencyIssueId", index1);
            row.OrchestrationPlan = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "OrchestrationPlanId", index5);
            row.Predecessor = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "PredecessorId", index15);
            row.Successor = global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequiredTarget(source, "SuccessorId", index15);
        }

        return typed;
    }

    public global::Meta.Operations.Domain.InMemoryWorkspace Write(global::MetaOrchestration.MetaOrchestrationModel model, global::Meta.Operations.Domain.GenericModel contract)
    {
        global::System.ArgumentNullException.ThrowIfNull(model);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireContract(contract, ContractSignature);
        var instance = new global::Meta.Operations.Domain.GenericInstance
        {
            ModelName = contract.Name
        };
        var index0 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DataObjectList, row => row.Id, "DataObject");
        var index1 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DependencyIssueList, row => row.Id, "DependencyIssue");
        var index2 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.DependencyIssuePipelineList, row => row.Id, "DependencyIssuePipeline");
        var index3 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.LockCompatibilityPolicyList, row => row.Id, "LockCompatibilityPolicy");
        var index4 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.ObjectAccessList, row => row.Id, "ObjectAccess");
        var index5 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.OrchestrationPlanList, row => row.Id, "OrchestrationPlan");
        var index6 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PipelineDependencyList, row => row.Id, "PipelineDependency");
        var index7 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PipelineObjectAccessList, row => row.Id, "PipelineObjectAccess");
        var index8 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PipelineReferenceList, row => row.Id, "PipelineReference");
        var index9 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PlannedTaskList, row => row.Id, "PlannedTask");
        var index10 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.PlannedTaskLockList, row => row.Id, "PlannedTaskLock");
        var index11 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RetryPolicyList, row => row.Id, "RetryPolicy");
        var index12 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RetryPolicyFailureClassList, row => row.Id, "RetryPolicyFailureClass");
        var index13 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RunPlanList, row => row.Id, "RunPlan");
        var index14 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.RunPlanRetryPolicyList, row => row.Id, "RunPlanRetryPolicy");
        var index15 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TaskAccessProfileList, row => row.Id, "TaskAccessProfile");
        var index16 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TaskDependencyList, row => row.Id, "TaskDependency");
        var index17 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TaskObjectEffectList, row => row.Id, "TaskObjectEffect");
        var index18 = global::Meta.TypedModels.TypedWorkspaceBindingValidation.Index(model.TaskOrderingResolutionList, row => row.Id, "TaskOrderingResolution");
        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index0))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.NormalizedKey is not null)
                target.Values.Add("NormalizedKey", row.NormalizedKey);
            if (row.SqlIdentifier is not null)
                target.Values.Add("SqlIdentifier", row.SqlIdentifier);
            if (row.OrchestrationPlan is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.OrchestrationPlan, row.OrchestrationPlan.Id, index5, "DataObject.OrchestrationPlan");
                target.RelationshipIds.Add("OrchestrationPlanId", row.OrchestrationPlan.Id);
            }

            instance.GetOrCreateEntityRecords("DataObject").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index1))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.BlocksAutomaticRunPlanning is not null)
                target.Values.Add("BlocksAutomaticRunPlanning", row.BlocksAutomaticRunPlanning);
            if (row.BlocksDag is not null)
                target.Values.Add("BlocksDag", row.BlocksDag);
            if (row.Code is not null)
                target.Values.Add("Code", row.Code);
            if (row.IssueDomain is not null)
                target.Values.Add("IssueDomain", row.IssueDomain);
            if (row.Message is not null)
                target.Values.Add("Message", row.Message);
            if (row.Severity is not null)
                target.Values.Add("Severity", row.Severity);
            if (row.DataObject is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DataObject, row.DataObject.Id, index0, "DependencyIssue.DataObject");
                target.RelationshipIds.Add("DataObjectId", row.DataObject.Id);
            }

            if (row.OrchestrationPlan is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.OrchestrationPlan, row.OrchestrationPlan.Id, index5, "DependencyIssue.OrchestrationPlan");
                target.RelationshipIds.Add("OrchestrationPlanId", row.OrchestrationPlan.Id);
            }

            instance.GetOrCreateEntityRecords("DependencyIssue").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index2))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Role is not null)
                target.Values.Add("Role", row.Role);
            if (row.DependencyIssue is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DependencyIssue, row.DependencyIssue.Id, index1, "DependencyIssuePipeline.DependencyIssue");
                target.RelationshipIds.Add("DependencyIssueId", row.DependencyIssue.Id);
            }

            if (row.PipelineReference is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PipelineReference, row.PipelineReference.Id, index8, "DependencyIssuePipeline.PipelineReference");
                target.RelationshipIds.Add("PipelineReferenceId", row.PipelineReference.Id);
            }

            instance.GetOrCreateEntityRecords("DependencyIssuePipeline").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index3))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LeftEffect is not null)
                target.Values.Add("LeftEffect", row.LeftEffect);
            if (row.LockBehavior is not null)
                target.Values.Add("LockBehavior", row.LockBehavior);
            if (row.PolicyKind is not null)
                target.Values.Add("PolicyKind", row.PolicyKind);
            if (row.Reason is not null)
                target.Values.Add("Reason", row.Reason);
            if (row.RightEffect is not null)
                target.Values.Add("RightEffect", row.RightEffect);
            if (row.Status is not null)
                target.Values.Add("Status", row.Status);
            if (row.DataObject is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DataObject, row.DataObject.Id, index0, "LockCompatibilityPolicy.DataObject");
                target.RelationshipIds.Add("DataObjectId", row.DataObject.Id);
            }

            if (row.DependencyIssue is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DependencyIssue, row.DependencyIssue.Id, index1, "LockCompatibilityPolicy.DependencyIssue");
                target.RelationshipIds.Add("DependencyIssueId", row.DependencyIssue.Id);
            }

            if (row.OrchestrationPlan is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.OrchestrationPlan, row.OrchestrationPlan.Id, index5, "LockCompatibilityPolicy.OrchestrationPlan");
                target.RelationshipIds.Add("OrchestrationPlanId", row.OrchestrationPlan.Id);
            }

            instance.GetOrCreateEntityRecords("LockCompatibilityPolicy").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index4))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.AccessKind is not null)
                target.Values.Add("AccessKind", row.AccessKind);
            if (row.AccessRole is not null)
                target.Values.Add("AccessRole", row.AccessRole);
            if (row.OperationKind is not null)
                target.Values.Add("OperationKind", row.OperationKind);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.Reason is not null)
                target.Values.Add("Reason", row.Reason);
            if (row.DataObject is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DataObject, row.DataObject.Id, index0, "ObjectAccess.DataObject");
                target.RelationshipIds.Add("DataObjectId", row.DataObject.Id);
            }

            if (row.TaskAccessProfile is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TaskAccessProfile, row.TaskAccessProfile.Id, index15, "ObjectAccess.TaskAccessProfile");
                target.RelationshipIds.Add("TaskAccessProfileId", row.TaskAccessProfile.Id);
            }

            instance.GetOrCreateEntityRecords("ObjectAccess").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index5))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DagStatus is not null)
                target.Values.Add("DagStatus", row.DagStatus);
            if (row.Description is not null)
                target.Values.Add("Description", row.Description);
            if (row.DeterminismStatus is not null)
                target.Values.Add("DeterminismStatus", row.DeterminismStatus);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.SynchronizationStatus is not null)
                target.Values.Add("SynchronizationStatus", row.SynchronizationStatus);
            instance.GetOrCreateEntityRecords("OrchestrationPlan").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index6))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DependencyKind is not null)
                target.Values.Add("DependencyKind", row.DependencyKind);
            if (row.Reason is not null)
                target.Values.Add("Reason", row.Reason);
            if (row.OrchestrationPlan is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.OrchestrationPlan, row.OrchestrationPlan.Id, index5, "PipelineDependency.OrchestrationPlan");
                target.RelationshipIds.Add("OrchestrationPlanId", row.OrchestrationPlan.Id);
            }

            if (row.Predecessor is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Predecessor, row.Predecessor.Id, index8, "PipelineDependency.Predecessor");
                target.RelationshipIds.Add("PredecessorId", row.Predecessor.Id);
            }

            if (row.Successor is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Successor, row.Successor.Id, index8, "PipelineDependency.Successor");
                target.RelationshipIds.Add("SuccessorId", row.Successor.Id);
            }

            instance.GetOrCreateEntityRecords("PipelineDependency").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index7))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.AccessKind is not null)
                target.Values.Add("AccessKind", row.AccessKind);
            if (row.Reason is not null)
                target.Values.Add("Reason", row.Reason);
            if (row.DataObject is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DataObject, row.DataObject.Id, index0, "PipelineObjectAccess.DataObject");
                target.RelationshipIds.Add("DataObjectId", row.DataObject.Id);
            }

            if (row.PipelineReference is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PipelineReference, row.PipelineReference.Id, index8, "PipelineObjectAccess.PipelineReference");
                target.RelationshipIds.Add("PipelineReferenceId", row.PipelineReference.Id);
            }

            instance.GetOrCreateEntityRecords("PipelineObjectAccess").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index8))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.MetaPipelinePipelineId is not null)
                target.Values.Add("MetaPipelinePipelineId", row.MetaPipelinePipelineId);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.PipelineWorkspacePath is not null)
                target.Values.Add("PipelineWorkspacePath", row.PipelineWorkspacePath);
            if (row.OrchestrationPlan is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.OrchestrationPlan, row.OrchestrationPlan.Id, index5, "PipelineReference.OrchestrationPlan");
                target.RelationshipIds.Add("OrchestrationPlanId", row.OrchestrationPlan.Id);
            }

            instance.GetOrCreateEntityRecords("PipelineReference").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index9))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.Reason is not null)
                target.Values.Add("Reason", row.Reason);
            if (row.PipelineReference is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PipelineReference, row.PipelineReference.Id, index8, "PlannedTask.PipelineReference");
                target.RelationshipIds.Add("PipelineReferenceId", row.PipelineReference.Id);
            }

            if (row.RunPlan is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.RunPlan, row.RunPlan.Id, index13, "PlannedTask.RunPlan");
                target.RelationshipIds.Add("RunPlanId", row.RunPlan.Id);
            }

            if (row.TaskAccessProfile is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TaskAccessProfile, row.TaskAccessProfile.Id, index15, "PlannedTask.TaskAccessProfile");
                target.RelationshipIds.Add("TaskAccessProfileId", row.TaskAccessProfile.Id);
            }

            instance.GetOrCreateEntityRecords("PlannedTask").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index10))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.LockMode is not null)
                target.Values.Add("LockMode", row.LockMode);
            if (row.Reason is not null)
                target.Values.Add("Reason", row.Reason);
            if (row.DataObject is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DataObject, row.DataObject.Id, index0, "PlannedTaskLock.DataObject");
                target.RelationshipIds.Add("DataObjectId", row.DataObject.Id);
            }

            if (row.LockCompatibilityPolicy is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.LockCompatibilityPolicy, row.LockCompatibilityPolicy.Id, index3, "PlannedTaskLock.LockCompatibilityPolicy");
                target.RelationshipIds.Add("LockCompatibilityPolicyId", row.LockCompatibilityPolicy.Id);
            }

            if (row.PlannedTask is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PlannedTask, row.PlannedTask.Id, index9, "PlannedTaskLock.PlannedTask");
                target.RelationshipIds.Add("PlannedTaskId", row.PlannedTask.Id);
            }

            if (row.TaskObjectEffect is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TaskObjectEffect, row.TaskObjectEffect.Id, index17, "PlannedTaskLock.TaskObjectEffect");
                target.RelationshipIds.Add("TaskObjectEffectId", row.TaskObjectEffect.Id);
            }

            instance.GetOrCreateEntityRecords("PlannedTaskLock").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index11))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.BackoffMultiplier is not null)
                target.Values.Add("BackoffMultiplier", row.BackoffMultiplier);
            if (row.InitialDelayMilliseconds is not null)
                target.Values.Add("InitialDelayMilliseconds", row.InitialDelayMilliseconds);
            if (row.MaxAttempts is not null)
                target.Values.Add("MaxAttempts", row.MaxAttempts);
            if (row.MaxDelayMilliseconds is not null)
                target.Values.Add("MaxDelayMilliseconds", row.MaxDelayMilliseconds);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.PolicyKind is not null)
                target.Values.Add("PolicyKind", row.PolicyKind);
            if (row.Reason is not null)
                target.Values.Add("Reason", row.Reason);
            if (row.RetryReadOnlyTasksByDefault is not null)
                target.Values.Add("RetryReadOnlyTasksByDefault", row.RetryReadOnlyTasksByDefault);
            if (row.RetryWriteTasksByDefault is not null)
                target.Values.Add("RetryWriteTasksByDefault", row.RetryWriteTasksByDefault);
            if (row.Status is not null)
                target.Values.Add("Status", row.Status);
            if (row.OrchestrationPlan is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.OrchestrationPlan, row.OrchestrationPlan.Id, index5, "RetryPolicy.OrchestrationPlan");
                target.RelationshipIds.Add("OrchestrationPlanId", row.OrchestrationPlan.Id);
            }

            instance.GetOrCreateEntityRecords("RetryPolicy").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index12))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.FailureClass is not null)
                target.Values.Add("FailureClass", row.FailureClass);
            if (row.Reason is not null)
                target.Values.Add("Reason", row.Reason);
            if (row.RetryBehavior is not null)
                target.Values.Add("RetryBehavior", row.RetryBehavior);
            if (row.RetryPolicy is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.RetryPolicy, row.RetryPolicy.Id, index11, "RetryPolicyFailureClass.RetryPolicy");
                target.RelationshipIds.Add("RetryPolicyId", row.RetryPolicy.Id);
            }

            instance.GetOrCreateEntityRecords("RetryPolicyFailureClass").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index13))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Reason is not null)
                target.Values.Add("Reason", row.Reason);
            if (row.RunPlanStatus is not null)
                target.Values.Add("RunPlanStatus", row.RunPlanStatus);
            if (row.OrchestrationPlan is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.OrchestrationPlan, row.OrchestrationPlan.Id, index5, "RunPlan.OrchestrationPlan");
                target.RelationshipIds.Add("OrchestrationPlanId", row.OrchestrationPlan.Id);
            }

            instance.GetOrCreateEntityRecords("RunPlan").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index14))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.PolicyRole is not null)
                target.Values.Add("PolicyRole", row.PolicyRole);
            if (row.Reason is not null)
                target.Values.Add("Reason", row.Reason);
            if (row.RetryPolicy is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.RetryPolicy, row.RetryPolicy.Id, index11, "RunPlanRetryPolicy.RetryPolicy");
                target.RelationshipIds.Add("RetryPolicyId", row.RetryPolicy.Id);
            }

            if (row.RunPlan is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.RunPlan, row.RunPlan.Id, index13, "RunPlanRetryPolicy.RunPlan");
                target.RelationshipIds.Add("RunPlanId", row.RunPlan.Id);
            }

            instance.GetOrCreateEntityRecords("RunPlanRetryPolicy").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index15))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.BindingWorkspacePath is not null)
                target.Values.Add("BindingWorkspacePath", row.BindingWorkspacePath);
            if (row.MetaPipelinePipelineTaskId is not null)
                target.Values.Add("MetaPipelinePipelineTaskId", row.MetaPipelinePipelineTaskId);
            if (row.Ordinal is not null)
                target.Values.Add("Ordinal", row.Ordinal);
            if (row.StatementKind is not null)
                target.Values.Add("StatementKind", row.StatementKind);
            if (row.TaskKind is not null)
                target.Values.Add("TaskKind", row.TaskKind);
            if (row.TaskName is not null)
                target.Values.Add("TaskName", row.TaskName);
            if (row.TransformBindingId is not null)
                target.Values.Add("TransformBindingId", row.TransformBindingId);
            if (row.TransformScriptId is not null)
                target.Values.Add("TransformScriptId", row.TransformScriptId);
            if (row.TransformScriptName is not null)
                target.Values.Add("TransformScriptName", row.TransformScriptName);
            if (row.TransformWorkspacePath is not null)
                target.Values.Add("TransformWorkspacePath", row.TransformWorkspacePath);
            if (row.PipelineReference is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.PipelineReference, row.PipelineReference.Id, index8, "TaskAccessProfile.PipelineReference");
                target.RelationshipIds.Add("PipelineReferenceId", row.PipelineReference.Id);
            }

            instance.GetOrCreateEntityRecords("TaskAccessProfile").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index16))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DependencyCondition is not null)
                target.Values.Add("DependencyCondition", row.DependencyCondition);
            if (row.DependencyKind is not null)
                target.Values.Add("DependencyKind", row.DependencyKind);
            if (row.Reason is not null)
                target.Values.Add("Reason", row.Reason);
            if (row.DataObject is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DataObject, row.DataObject.Id, index0, "TaskDependency.DataObject");
                target.RelationshipIds.Add("DataObjectId", row.DataObject.Id);
            }

            if (row.OrchestrationPlan is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.OrchestrationPlan, row.OrchestrationPlan.Id, index5, "TaskDependency.OrchestrationPlan");
                target.RelationshipIds.Add("OrchestrationPlanId", row.OrchestrationPlan.Id);
            }

            if (row.Predecessor is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Predecessor, row.Predecessor.Id, index15, "TaskDependency.Predecessor");
                target.RelationshipIds.Add("PredecessorId", row.Predecessor.Id);
            }

            if (row.Successor is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Successor, row.Successor.Id, index15, "TaskDependency.Successor");
                target.RelationshipIds.Add("SuccessorId", row.Successor.Id);
            }

            instance.GetOrCreateEntityRecords("TaskDependency").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index17))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.AccessDirection is not null)
                target.Values.Add("AccessDirection", row.AccessDirection);
            if (row.AccessPurpose is not null)
                target.Values.Add("AccessPurpose", row.AccessPurpose);
            if (row.CreatesDataDependency is not null)
                target.Values.Add("CreatesDataDependency", row.CreatesDataDependency);
            if (row.IsPublishedProducer is not null)
                target.Values.Add("IsPublishedProducer", row.IsPublishedProducer);
            if (row.LockMode is not null)
                target.Values.Add("LockMode", row.LockMode);
            if (row.Reason is not null)
                target.Values.Add("Reason", row.Reason);
            if (row.RequiresSynchronization is not null)
                target.Values.Add("RequiresSynchronization", row.RequiresSynchronization);
            if (row.WriteEffect is not null)
                target.Values.Add("WriteEffect", row.WriteEffect);
            if (row.DataObject is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DataObject, row.DataObject.Id, index0, "TaskObjectEffect.DataObject");
                target.RelationshipIds.Add("DataObjectId", row.DataObject.Id);
            }

            if (row.TaskAccessProfile is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.TaskAccessProfile, row.TaskAccessProfile.Id, index15, "TaskObjectEffect.TaskAccessProfile");
                target.RelationshipIds.Add("TaskAccessProfileId", row.TaskAccessProfile.Id);
            }

            instance.GetOrCreateEntityRecords("TaskObjectEffect").Add(target);
        }

        foreach (var row in global::Meta.TypedModels.TypedWorkspaceBindingValidation.Ordered(index18))
        {
            var target = new global::Meta.Operations.Domain.GenericRecord
            {
                Id = row.Id
            };
            if (row.DependencyCondition is not null)
                target.Values.Add("DependencyCondition", row.DependencyCondition);
            if (row.Name is not null)
                target.Values.Add("Name", row.Name);
            if (row.Reason is not null)
                target.Values.Add("Reason", row.Reason);
            if (row.ResolutionKind is not null)
                target.Values.Add("ResolutionKind", row.ResolutionKind);
            if (row.Status is not null)
                target.Values.Add("Status", row.Status);
            if (row.DataObject is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DataObject, row.DataObject.Id, index0, "TaskOrderingResolution.DataObject");
                target.RelationshipIds.Add("DataObjectId", row.DataObject.Id);
            }

            if (row.DependencyIssue is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.DependencyIssue, row.DependencyIssue.Id, index1, "TaskOrderingResolution.DependencyIssue");
                target.RelationshipIds.Add("DependencyIssueId", row.DependencyIssue.Id);
            }

            if (row.OrchestrationPlan is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.OrchestrationPlan, row.OrchestrationPlan.Id, index5, "TaskOrderingResolution.OrchestrationPlan");
                target.RelationshipIds.Add("OrchestrationPlanId", row.OrchestrationPlan.Id);
            }

            if (row.Predecessor is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Predecessor, row.Predecessor.Id, index15, "TaskOrderingResolution.Predecessor");
                target.RelationshipIds.Add("PredecessorId", row.Predecessor.Id);
            }

            if (row.Successor is not null)
            {
                global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireCanonical(row.Successor, row.Successor.Id, index15, "TaskOrderingResolution.Successor");
                target.RelationshipIds.Add("SuccessorId", row.Successor.Id);
            }

            instance.GetOrCreateEntityRecords("TaskOrderingResolution").Add(target);
        }

        var state = new global::Meta.Operations.Domain.InMemoryWorkspace(contract.Clone(), instance);
        global::Meta.TypedModels.TypedWorkspaceBindingValidation.RequireValid(state);
        return state;
    }
}