import React from "react"

interface LogoProps extends React.SVGProps<SVGSVGElement> {
  size?: number
}

export function Logo({ size = 36, className, ...props }: LogoProps) {
  return (
    <svg
      width={size}
      height={(size * 380) / 320}
      viewBox="0 0 320 380"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
      className={className}
      {...props}
    >
      {/* Outer Frame */}
      <rect
        x="60"
        y="40"
        width="200"
        height="320"
        rx="44"
        stroke="#EF8C34"
        strokeWidth="12"
        fill="none"
      />

      {/* Upward Triangle Arrow */}
      <polygon
        points="160,115 135,150 185,150"
        fill="#EF8C34"
        stroke="#EF8C34"
        strokeWidth="6"
        strokeLinejoin="round"
      />

      {/* Left Door */}
      <path
        d="M 157 170 L 120 170 A 20 20 0 0 0 100 190 L 100 270 A 20 20 0 0 0 120 290 L 157 290 Z"
        fill="#EF8C34"
      />
      {/* Cut-out window handle matching page background dynamically */}
      <circle
        cx="132"
        cy="230"
        r="10"
        fill="currentColor"
        className="text-background"
      />

      {/* Right Door */}
      <path
        d="M 163 170 L 200 170 A 20 20 0 0 1 220 190 L 220 270 A 20 20 0 0 1 200 290 L 163 290 Z"
        fill="#EF8C34"
      />
      {/* Cut-out window handle matching page background dynamically */}
      <circle
        cx="188"
        cy="230"
        r="10"
        fill="currentColor"
        className="text-background"
      />

      {/* Bottom Horizontal Line */}
      <line
        x1="0"
        y1="360"
        x2="320"
        y2="360"
        stroke="#EF8C34"
        strokeWidth="12"
        strokeLinecap="round"
      />

      {/* Bottom Semicircle Notch */}
      <path
        d="M 140 360 A 20 20 0 0 0 180 360 Z"
        fill="#EF8C34"
      />
    </svg>
  )
}
