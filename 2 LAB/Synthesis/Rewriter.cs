namespace Synthesis
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class Rewriter : CSharpSyntaxRewriter
    {
       
        Hashtable lista = new Hashtable();
        LiteralExpressionSyntax poRet;
        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {

            if(!node.ChildNodes().OfType<PredefinedTypeSyntax>().FirstOrDefault().ToString().Equals("void"))
            {
                var wynikMetody = node.ChildNodes().OfType<BlockSyntax>().FirstOrDefault()
                .ChildNodes().OfType<ReturnStatementSyntax>().FirstOrDefault();
                //Console.WriteLine(wynikMetody.ToString());
                var poReturnie = wynikMetody.ChildNodes().OfType<LiteralExpressionSyntax>().FirstOrDefault();
                poRet = poReturnie;
                //Console.WriteLine(poReturnie.ToString());

                if (poReturnie != null)
                {
                    lista.Add(node.Identifier.ToString(), poReturnie);
                    //  Console.WriteLine(lista[node.Identifier]);
                    
                }
            }

            return base.VisitMethodDeclaration(node);
        }

        public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            //Console.WriteLine(lista["Cztery"]);

            var New = node;

            var pobierzLinie = node.ChildNodes().OfType<VariableDeclarationSyntax>().FirstOrDefault();
           // Console.WriteLine(pobierzLinie.ToString());

            var typZmiennej = pobierzLinie.ChildNodes().OfType<PredefinedTypeSyntax>().FirstOrDefault();
            //Console.WriteLine(typZmiennej);

            String typ = typZmiennej + " ";
            String nazwa = pobierzLinie.ChildNodes().OfType<VariableDeclaratorSyntax>().FirstOrDefault().Identifier.ToString() + " = ";
            
            var tmp = pobierzLinie.ChildNodes().OfType<VariableDeclaratorSyntax>().FirstOrDefault();
            Console.WriteLine(tmp.ToString());
            if (typZmiennej.ToString().Equals("int") || typZmiennej.ToString().Equals("string")) { 

            var wyjscieZmiennej = pobierzLinie.ChildNodes().OfType<VariableDeclaratorSyntax>().FirstOrDefault()
                .ChildNodes().OfType<EqualsValueClauseSyntax>().FirstOrDefault()
                .ChildNodes().OfType<InvocationExpressionSyntax>().FirstOrDefault();

                var nazwaMetody = wyjscieZmiennej.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault().ToString();
               // Console.WriteLine(wyjscieZmiennej);
               // Console.WriteLine(nazwaMetody);
             

                var pobierzNawias = wyjscieZmiennej.ChildNodes().OfType<ArgumentListSyntax>().FirstOrDefault().ToString();
                //Console.WriteLine(pobierzNawias.ToString());

                if (pobierzNawias.ToString().Equals("()"))
            {

                    

                    if (lista.ContainsKey(wyjscieZmiennej.ToString().Substring(0,wyjscieZmiennej.ToString().Length-2)))
                    {
                        String prawo = lista[nazwaMetody] + ";";
                        var cale = typ + nazwa + prawo;
                      
                      
                        
                       var NowePole = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(typZmiennej).
                           WithVariables(SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(tmp.WithInitializer(SyntaxFactory.
                           EqualsValueClause(poRet).WithEqualsToken(SyntaxFactory.Token(SyntaxFactory.TriviaList(),SyntaxKind.
                           EqualsToken,SyntaxFactory.TriviaList(SyntaxFactory.Space))))))).WithSemicolonToken(SyntaxFactory.
                           Token(SyntaxFactory.TriviaList(),SyntaxKind.SemicolonToken,SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)));
                       
                        node = node.ReplaceNode(node, NowePole);
                        
                 
                    }
                    
                    
            }

            }

            return base.VisitFieldDeclaration(node);
        }


       

    }





    

}
