<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Default.aspx.vb" Inherits="ProCompIdentity._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>ProComp Identity Demonstration</h1>
        <p class="lead">
            This project demonstrates a simple, custom implementation of the Identity
                framework.
        </p>
        <p>
            To get started, click Register to create an account. After registering
                or logging in, your name should appear in the navigation bar where
                the Register link used to be. Click it to view information about
                your account.
        </p>
    </div>

    <div>
        <h2>Explanation</h2>
        <p>Here are some guidelines to understand what the code is doing and how.</p>
        <p>
            Try out the <code>Account/Manage</code> page to see your identity's claims and to toggle
            the admin role to demonstrate role-based authentication.
        </p>

        <h2>Frameworks Used</h2>
        <h3>OWIN</h3>
        <p>
            In any in web application there are typically two major processes participating:
                the web server and web application. The server waits for HTTP requests
                and sends back response. If the request is for a static file like
                an image, the web server handles it without any outside help. If
                the request is for something dynamic, the web server delegates the
                request to the web application. The web application receives a request
                from the web server and dynamically generates a response that it returns
                to the server, which then passes it back to the browser.
        </p>
        <p>
            Communication between the server and the application is complicated and must be customized
                for each server / application pair. The OWIN (Open Web INterface)
                framework attempts to simplify this by creating a single, unified
                method of communication between servers and applications. It allows
                an ASP.NET website to be hosted in IIS, a test server or any OWIN-compliant
                server without having to rewrite the application.
        </p>
        <p>
            See more at <a href="http://owin.org/">http://owin.org/</a>
        </p>

        <h3>Identity</h3>
        <p>
            ASP.NET Identity works with OWIN to help the web application handle authentication
                and authorization of requests. The framework itself is meant to be
                extremely generic so it can be used with any app. Microsoft provides
                a default implementation using Entity framework that works well for
                new / simple applications, but anything that has complex requirements
                for authentication and an existing user base is better off with a
                custom implementation, which is what this site demonstrates.
        </p>
        <p>
            See more at <a href="http://www.asp.et/identity/overview/getting-started/introduction-to-aspnet-identity">http://www.asp.net/identity/overview/getting-started/introduction-to-aspnet-identity</a>
        </p>

        <h3>Claims-based Identity</h3>
        <p>
            Claims-based Identity is not a framework itself but a modern concept of authentication
                and authorization. A user's identity is created from a collection
                of claims asserted by an issuer trusted by the application. Claims
                can come from multiple sources if necessary, and in particular this
                means an application can delegate authentication to a 3rd party like
                Google or Facebook when it makes sense.
        </p>
        <p>
            As an example, an application can rely on Google to authenticate a user. Many people
                already have Google accounts secured by 2-factor authentication.
                By asking users to sign in with their Google account, the application
                can trust Google to handle the account creation and verification
                process and return a set of claims, such as the user's name and email
                address. The application can then associate data with that identity.
        </p>
        <p>
            Of course in ProComp's case, instead of using an external claims provider there could
                be a centralized server, controlled by you, that handles user authentication
                and claims based on your existing user data. Your other applications
                would then use it as a service to log users in and get information
                about them. Or, as is the case with this demonstration site, it can
                just be built right into the application.
        </p>
        <p>
            See more at <a href="https://en.wikipedia.org/wiki/Claims-based_identity">https://en.wikipedia.org/wiki/Claims-based_identity</a>
        </p>

        <h2>Site Startup</h2>
        <p>
            On startup, <code>App_Start/Startup.Auth.vb</code> runs <code>ConfigureAuth</code>,
                configuring the application to use cookie authentication and the
                custom <code>ProCompUserManager</code>. The user manager class is
                responsible managing user identities and the cookies help the
                browser remember the user between requests since HTTP is stateless.
        </p>
        <p>
            <code>CreatePerOwinContext</code> is part of OWIN's request lifecycle
                management and allows pages to ensure that a single instance of an
                object is created per request and can be access by any code using
                the current <code>IOwinContext</code>.
        </p>
        <h2>Custom Implementation</h2>
        <h3>ProCompMembershipService</h3>
        <p>
            Although not strictly necessary, this class simplifies the tasks related to logging
                users in an out, getting the current user or registering a new user.
        </p>

        <h3>ProCompPasswordHasher</h3>
        <p>
            This class implements the <code>IPasswordHasher</code> interface,
                giving your application complete control over how user passwords
                are hashed. If you have an existing user base that maybe doesn't
                hash passwords, you can create an implementation to support that
                and then swap implementations later to toughen up your app against
                hackers.
        </p>

        <h3>ProCompSignInManager</h3>
        <p>
            The class subclasses <code>SignInManager</code>, which helps your
                application log users in. The only real purpose of the subclass is
                to let the framework know where your users come from, which, in this
                case, is <code>ProCompUserManager</code>.
        </p>
        <p>
            Although it looks underwhelming, the class is helpful when your app uses
                an external service for authentication, or uses a custom implementation
                of 2-factor authentication.
        </p>
        <p>
            It also does some nice things for free, like checking to see if a user
                account has been locked after too many failed login attempts, if
                your application opts in to that feature.
        </p>

        <h3>ProCompUser</h3>
        <p>
            This class implements <code>IUser</code>, a simple interface needed
                by the framework to represent users. At a bare minimum, every user
                must have an Id and a UserName. Beyond that, your user class can
                have as many properties as you want, called anything you want.
            Properties related to Identity stuff will be accessed through interfaces
            your user store implements. See <code>ProCompJsonUserStore</code> below.
        </p>
        <p>
            In this example implementation, <code>ProCompUser</code> bridges the
                gap between the Identity framework and our simplistic user model,
                <code>UserEntry</code>. For demo purposes, we have a very simple
                user class and we get users from a JSON file but a real application
                would likely have a much more detailed user model that would be fetched
                from a SQL Server database.
        </p>

        <h3>ProCompUserManager</h3>
        <p>
            This class subclasses <code>UserManager</code>, a built-in Identity
                framework class that provides all sort of user management code for
                free. <code>UserManager</code> can create new user accounts and will
                automatically check for existing users with the same name, verify
                that the user's password meets requirements, hash the password for
                you, and store the new user in your storage of choice. It will even
                email users to let them know their account has been created and needs
                to be verified if your application requires that. This class can
                be used to add or a remove a user from a role and check whether the
                user belong to a role.
        </p>
        <p>
            This subclass exists to configure the class to do things the way we like,
                using our rules for username and password validity, whether users
                should be able to become locked out and under what conditions, and
                how to hash passwords.
        </p>
        <p>
            Of course, the <code>UserManager</code> isn't magic and it relies on
                <code>ProCompJsonUserStore</code> to actually manipulate users.
        </p>

        <h3>ProCompJsonUserStore</h3>
        <p>
            This is the big one. This class is the main gateway between the Identity framework
                and our user storage medium. The class implements a large number
                of interfaces, each providing a bit of the behavior that we opt into.
        </p>
        <p>
            This is how the framework can be adapted to any application with any
                domain model and storage medium. We only need to provide the properties
                and methods for the features we want to support and the <code>UserManager</code>
            class will do the rest of the work.
        </p>

        <h4>IUserStore</h4>
        <p>
            This is the main <strong>CRUD</strong> interface for user accounts. Implementing
                this interface allows the framework to Create, Find, Update and Delete
                users.
        </p>
        <p>
            For demonstration purposes, all we do is read and write a JSON file that
                stores users with a few basic properties.
        </p>

        <h4>IUserPasswordStore</h4>
        <p>
            Implementing this interface allows the framework to get, set and check on
                the password property of our custom user class, assuming we require
                users to have a password.
        </p>

        <h4>IUserLockoutStore</h4>
        <p>
            Implementing this interface allows the framework to keep track of failed
                user login attempts and determine whether or not a user should be
                locked out.
        </p>
        <p>
            If our app needs this feature, we need to provide the properties to keep
                track of the information in our user model (<code>LockoutEndDate</code>,
                <code>FailedLoginAttemps</code>) and the methods to manipulate them
                (<code>GetLockoutEndDateAsync</code>, <code>IncrementAccessFailedCountAsync</code>,
                etc.)
        </p>
        <p>
            In the demo example, we allow users to become locked out unless they belong to the admin
            role. Maybe a poor choice in the real world but it shows how behavior can be customized.
        </p>

        <h4>IUserEmailStore</h4>
        <p>
            Implementing this interface allows the framework to associate an email
                address with users, which it can use to make users verify their accounts
                after creation, or let them reset their password.
        </p>

        <h4>IUserTwoFactorStore</h4>
        <p>
            Implementing this interface allows the framework to handle 2-factor authentication
                for users. In this example, we simply tell the framework that it's
                never enabled.
        </p>

        <h4>IUserRoleStore</h4>
        <p>
            Implementing this interface allows the framework to associate users with
                roles. We can add and remove users from roles, get the set of roles
                user belongs to or just ask if a user belongs to a role.
        </p>
        <p>
            Because the concept of roles is integrated into ASP.NET, we can grant
                or deny users access to specific pages or folders based on role membership.
                To see an example, check the <code>Account/Web.config</code> file.
        </p>
        <p>
            ASP.NET also provides built-in controls that can display content based
                on a user's role, or based on whether or not they've been authenticated
                at all.
        </p>
    </div>
</asp:Content>
