﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.VisualStudio.ProjectSystem.VS.Properties.InterceptedProjectProperties;
using Microsoft.VisualStudio.ProjectSystem.VS.Utilities;
using Xunit;

namespace Microsoft.VisualStudio.ProjectSystem.VS.Properties
{
    [ProjectSystemTrait]
    public class PostBuildEventValueProviderTests
    {
        private static PostBuildEventValueProvider.PostBuildEventHelper systemUnderTest =
            new PostBuildEventValueProvider.PostBuildEventHelper();

        [Fact]
        public static void GetPropertyTest_AllTargetsPresent()
        {
            var root = @"
<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>

  <Target Name=""PreBuild"" BeforeTargets=""PreBuildEvent"">
    <Exec Command=""echo &quot;prebuild output&quot;"" />
  </Target>

  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build output&quot;"" />
  </Target>

</Project>
".AsProjectRootElement();
            var actual = systemUnderTest.GetProperty(root);
            Assert.Equal(@"echo ""post build output""", actual);
        }

        [Fact]
        public static void GetPropertyTest_PostBuildTargetPresent()
        {
            var root = @"
<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>

  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build output&quot;"" />
  </Target>

</Project>
".AsProjectRootElement();
            var actual = systemUnderTest.GetProperty(root);
            Assert.Equal(@"echo ""post build output""", actual);
        }

        [Fact]
        public static void GetPropertyTest_PostBuildTargetPresent_LowerCase()
        {
            var root = @"
<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>

  <Target Name=""postbuild"" AfterTargets=""postbuildevent"">
    <Exec Command=""echo &quot;post build output&quot;"" />
  </Target>

</Project>
".AsProjectRootElement();
            var actual = systemUnderTest.GetProperty(root);
            Assert.Equal(@"echo ""post build output""", actual);
        }

        [Fact]
        public static void GetPropertyTest_NoTargetsPresent()
        {
            var root = @"
<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>

</Project>
".AsProjectRootElement();
            var actual = systemUnderTest.GetProperty(root);
            Assert.Null(actual);
        }

        [Fact]
        public static void GetPropertyTest_ExistingProperties()
        {
            var root = @"
<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
    <PostBuildEvent>echo $(ProjectDir)</PostBuildEvent>
  </PropertyGroup>

</Project>
".AsProjectRootElement();
            var actual = systemUnderTest.GetProperty(root);
            Assert.Equal(@"echo $(ProjectDir)", actual);
        }

        [Fact]
        public static void SetPropertyTest_NoTargetsPresent()
        {
            var root = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
</Project>".AsProjectRootElement();
            systemUnderTest.SetProperty(@"echo ""post build output""", root);

            var expected = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build output&quot;"" />
  </Target>
</Project>";

            var actual = root.SaveAndGetChanges();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void SetPropertyTest_TargetPresent()
        {
            var root = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build output&quot;"" />
  </Target>
</Project>".AsProjectRootElement();
            systemUnderTest.SetProperty(@"echo ""post build $(OutDir)""", root);

            var expected = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build $(OutDir)&quot;"" />
  </Target>
</Project>";

            var actual = root.SaveAndGetChanges();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void SetPropertyTest_TargetPresent_LowerCase()
        {
            var root = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""postbuild"" AfterTargets=""postbuildevent"">
    <Exec Command=""echo &quot;post build output&quot;"" />
  </Target>
</Project>".AsProjectRootElement();
            systemUnderTest.SetProperty(@"echo ""post build $(OutDir)""", root);

            var expected = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""postbuild"" AfterTargets=""postbuildevent"">
    <Exec Command=""echo &quot;post build $(OutDir)&quot;"" />
  </Target>
</Project>";

            var actual = root.SaveAndGetChanges();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void SetPropertyTest_TargetPresent_NoTasks()
        {
            var root = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
  </Target>
</Project>".AsProjectRootElement();
            systemUnderTest.SetProperty(@"echo ""post build $(OutDir)""", root);

            var expected = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
  </Target>
  <Target Name=""PostBuild1"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build $(OutDir)&quot;"" />
  </Target>
</Project>";

            var actual = root.SaveAndGetChanges();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void SetPropertyTest_TargetPresent_NoTasks_Removal()
        {
            var root = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
  </Target>
  <Target Name=""PostBuild1"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build $(OutDir)&quot;"" />
  </Target>
</Project>".AsProjectRootElement();
            systemUnderTest.SetProperty(@"", root);

            var expected = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
  </Target>
</Project>";

            var actual = root.SaveAndGetChanges();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void SetPropertyTest_TargetPresent_MultipleTasks()
        {
            var root = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build output&quot;"" />
    <Exec Command=""echo &quot;post build output&quot;"" />
  </Target>
</Project>".AsProjectRootElement();
            systemUnderTest.SetProperty(@"echo ""post build $(OutDir)""", root);

            var expected = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build $(OutDir)&quot;"" />
    <Exec Command=""echo &quot;post build output&quot;"" />
  </Target>
</Project>";

            var actual = root.SaveAndGetChanges();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void SetPropertyTest_DoNotRemoveTarget_EmptyString()
        {
            var root = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build output&quot;"" />
    <Exec Command=""echo &quot;post build output&quot;"" />
  </Target>
</Project>".AsProjectRootElement();
            systemUnderTest.SetProperty(string.Empty, root);

            var expected = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Exec Command="""" />
    <Exec Command=""echo &quot;post build output&quot;"" />
  </Target>
</Project>";

            var actual = root.SaveAndGetChanges();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void SetPropertyTest_RemoveTarget_EmptyString()
        {
            var root = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build output&quot;"" />
  </Target>
</Project>".AsProjectRootElement();
            systemUnderTest.SetProperty(string.Empty, root);

            var expected = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
</Project>";

            var actual = root.SaveAndGetChanges();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void SetPropertyTest_RemoveTarget_WhitespaceCharacter()
        {
            var root = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build output&quot;"" />
  </Target>
</Project>".AsProjectRootElement();
            systemUnderTest.SetProperty("       ", root);

            var expected = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
</Project>";

            var actual = root.SaveAndGetChanges();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void SetPropertyTest_RemoveTarget_TabCharacter()
        {
            var root = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build output&quot;"" />
  </Target>
</Project>".AsProjectRootElement();
            systemUnderTest.SetProperty("\t\t\t", root);

            var expected = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
</Project>";

            var actual = root.SaveAndGetChanges();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void SetPropertyTest_DoNotRemoveTarget_NewlineCharacter()
        {
            var root = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build output&quot;"" />
  </Target>
</Project>".AsProjectRootElement();
            systemUnderTest.SetProperty("\r\n", root);

            var expected = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""&#xD;&#xA;"" />
  </Target>
</Project>";

            var actual = root.SaveAndGetChanges();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void SetPropertyTest_TargetNameCollision()
        {
            var root = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"">
  </Target>
</Project>".AsProjectRootElement();
            systemUnderTest.SetProperty(@"echo ""post build $(OutDir)""", root);

            var expected = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"">
  </Target>
  <Target Name=""PostBuild1"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build $(OutDir)&quot;"" />
  </Target>
</Project>";

            var actual = root.SaveAndGetChanges();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void SetPropertyTest_TargetNameCollision02()
        {
            var root = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"">
  </Target>
  <Target Name=""PostBuild1"">
  </Target>
</Project>
".AsProjectRootElement();
            systemUnderTest.SetProperty(@"echo ""post build $(OutDir)""", root);

            var expected = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""PostBuild"">
  </Target>
  <Target Name=""PostBuild1"">
  </Target>
  <Target Name=""PostBuild2"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build $(OutDir)&quot;"" />
  </Target>
</Project>";

            var actual = root.SaveAndGetChanges();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void SetPropertyTest_TargetNameCollision_LowerCase()
        {
            var root = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""postBuild"">
  </Target>
  <Target Name=""postBuild1"">
  </Target>
</Project>
".AsProjectRootElement();
            systemUnderTest.SetProperty(@"echo ""post build $(OutDir)""", root);

            var expected = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
  </PropertyGroup>
  <Target Name=""postBuild"">
  </Target>
  <Target Name=""postBuild1"">
  </Target>
  <Target Name=""PostBuild2"" AfterTargets=""PostBuildEvent"">
    <Exec Command=""echo &quot;post build $(OutDir)&quot;"" />
  </Target>
</Project>";

            var actual = root.SaveAndGetChanges();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public static void SetPropertyTest_ExistingProperties()
        {
            var root = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
    <PreBuildEvent>echo $(ProjectDir)</PreBuildEvent>
    <PostBuildEvent>echo $(ProjectDir)</PostBuildEvent>
  </PropertyGroup>
</Project>".AsProjectRootElement();
            systemUnderTest.SetProperty(@"echo ""post build $(OutDir)""", root);

            var expected = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>netcoreapp1.1</TargetFramework>
    <PreBuildEvent>echo $(ProjectDir)</PreBuildEvent>
    <PostBuildEvent>echo ""post build $(OutDir)""</PostBuildEvent>
  </PropertyGroup>
</Project>";

            var actual = root.SaveAndGetChanges();
            Assert.Equal(expected, actual);
        }
    }
}
