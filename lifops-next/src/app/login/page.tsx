import { LoginForm } from "@/components/auth/login-form"
import { Logo } from "@/components/marketing/logo"

export default function LoginPage() {
  return (
    <div className="container relative min-h-screen flex-col items-center justify-center grid lg:max-w-none lg:grid-cols-2 lg:px-0">
      <div className="relative hidden h-full flex-col bg-muted p-10 text-white dark:border-r lg:flex">
        <div className="absolute inset-0 bg-zinc-950" />
        <div className="absolute inset-0 bg-gradient-to-t from-zinc-900 to-zinc-900/50" />
        <div
          className="absolute inset-0 opacity-20"
          style={{
            backgroundImage: 'url("/abstract-geometric-shapes.png")',
            backgroundSize: "cover",
          }}
        />
        <div className="relative z-20 flex items-center gap-2.5 text-lg font-black tracking-tight">
          <Logo size={24} />
          <span>
            <span className="text-white">Lift</span>
            <span className="text-accent">Ops</span>
          </span>
        </div>
        <div className="relative z-20 mt-auto">
          <blockquote className="space-y-2">
            <p className="text-lg">
              "The most reliable management system for vertical transportation infrastructure. Built for speed, safety,
              and precision."
            </p>
            <footer className="text-sm text-zinc-400">Operations Control Center</footer>
          </blockquote>
        </div>
      </div>
      <div className="lg:p-8">
        <div className="mx-auto flex w-full flex-col justify-center space-y-6 sm:w-[350px]">
          <div className="flex flex-col space-y-2 text-center lg:hidden items-center">
            <Logo size={42} className="mb-2" />
            <h1 className="text-2xl font-black tracking-tight">
              <span className="text-foreground">Lift</span>
              <span className="text-accent">Ops</span>
            </h1>
          </div>
          <LoginForm />
          <p className="px-8 text-center text-sm text-muted-foreground">
            Platform operator?{" "}
            <a href="/admin/login" className="text-primary underline-offset-4 hover:underline">
              Platform admin sign-in
            </a>
          </p>
          <p className="px-8 text-center text-sm text-muted-foreground">
            By clicking continue, you agree to our{" "}
            <a href="#" className="underline underline-offset-4 hover:text-primary">
              Terms of Service
            </a>{" "}
            and{" "}
            <a href="#" className="underline underline-offset-4 hover:text-primary">
              Privacy Policy
            </a>
            .
          </p>
        </div>
      </div>
    </div>
  )
}
