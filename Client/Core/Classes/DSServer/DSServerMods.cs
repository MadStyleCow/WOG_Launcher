﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.7905
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 

public partial class DSServer {
    
    private DSServerMods[] modsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Mods")]
    public DSServerMods[] Mods {
        get {
            return this.modsField;
        }
        set {
            this.modsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://tempuri.org/DSServer.xsd")]
public partial class DSServerMods {
    
    private string nameField;
    
    private int typeField;
    
    private string webPageField;
    
    private int sequenceIDField;
    
    public DSServerMods() {
        this.typeField = 0;
        this.sequenceIDField = 0;
    }
    
    /// <remarks/>
    public string Name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    [System.ComponentModel.DefaultValueAttribute(0)]
    public int Type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
    
    /// <remarks/>
    public string WebPage {
        get {
            return this.webPageField;
        }
        set {
            this.webPageField = value;
        }
    }
    
    /// <remarks/>
    [System.ComponentModel.DefaultValueAttribute(0)]
    public int SequenceID {
        get {
            return this.sequenceIDField;
        }
        set {
            this.sequenceIDField = value;
        }
    }
}
