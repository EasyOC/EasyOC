using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;

namespace TransformalizeModule.Models {
   public class TransformalizeFormPart : ContentPart {

      public TransformalizeFormPart() {
         Arrangement = new TextField { Text = @"<cfg name=""Form"">

  <parameters>
    
    <!-- primary key must be defined here as an integer - default it to zero to indicate a new submission -->
    <add name=""FormId"" type=""int"" value=""0"" label=""Id"" primary-key=""true"" />
    
    <!-- if you plan to mark things as deleted, add this field on insert with a default of false (not deleted) -->
    <add name=""Deleted"" type=""bool"" value=""false"" input=""false"" scope=""insert"" />
    
    <!-- if you want to audit who created a record on insert -->
    <add name=""Created"" type=""datetime"" input=""false"" t=""now()"" scope=""insert"" format=""o"" />
    <add name=""CreatedBy"" type=""string"" input=""false"" t=""username()"" scope=""insert"" label=""Created By""  />
    
    <!-- if you want to audit who updated a record on update -->
    <add name=""Updated"" type=""datetime"" input=""false"" t=""now()"" scope=""update"" format=""o"" />
    <add name=""UpdatedBy"" type=""string"" input=""false"" t=""username()"" scope=""update"" label=""Updated By"" />

    <!-- your custom fields are defined here -->
    <!-- hint: set prompt to true to prompt the user -->
    <!-- hint: set label and hint as necessary -->
    <!-- hint: use type and input-type to control user input -->
    <!-- hint: use transforms and validation to control user input -->
  
  </parameters>

  <connections>
    <!-- hint: define a connection here or use one from common settings (e.g. Transformalize > Settings) -->
    <!-- hint: if the table does not already exist, you'll have to create it yourself -->
    <add name=""connection-name"" table=""table-name"" />
  </connections>

</cfg>" };

         LocationEnableHighAccuracy = new BooleanField() { Value = false };
         LocationMaximumAge = new NumericField() { Value = 0 };
         LocationTimeout = new NumericField() { Value = -1 };
      }
      public TextField Arrangement { get; set; }
      public BooleanField LocationEnableHighAccuracy { get; set; }
      public NumericField LocationMaximumAge { get; set; }
      public NumericField LocationTimeout { get; set; }
   }
}
