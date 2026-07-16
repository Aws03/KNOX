import type { SVGAttributes } from "react";

const Logo = (props: SVGAttributes<SVGElement>) => {
  return (
    <svg
      width="48"
      height="48"
      viewBox="0 0 200 200"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
      {...props}
    >
      <defs>
        <linearGradient id="knoxGradient" x1="0%" y1="0%" x2="100%" y2="100%">
          <stop offset="0%" stopColor="hsl(var(--primary))" stopOpacity="1" />
          <stop
            offset="100%"
            stopColor="hsl(var(--primary))"
            stopOpacity="0.8"
          />
        </linearGradient>
        <filter id="shadow">
          <feDropShadow dx="0" dy="2" stdDeviation="3" floodOpacity="0.2" />
        </filter>
      </defs>

      {/* Rounded square background with primary color */}
      <rect
        x="20"
        y="20"
        width="160"
        height="160"
        rx="32"
        fill="url(#knoxGradient)"
        filter="url(#shadow)"
      />

      {/* Subtle inner border */}
      <rect
        x="25"
        y="25"
        width="150"
        height="150"
        rx="28"
        fill="none"
        stroke="white"
        strokeWidth="1.5"
        opacity="0.2"
      />

      {/* Letter K */}
      <g>
        {/* K - vertical line */}
        <line
          x1="60"
          y1="65"
          x2="60"
          y2="135"
          stroke="white"
          strokeWidth="14"
          strokeLinecap="round"
        />
        {/* K - upper diagonal */}
        <line
          x1="60"
          y1="95"
          x2="95"
          y2="65"
          stroke="white"
          strokeWidth="14"
          strokeLinecap="round"
        />
        {/* K - lower diagonal */}
        <line
          x1="60"
          y1="105"
          x2="95"
          y2="135"
          stroke="white"
          strokeWidth="14"
          strokeLinecap="round"
        />
      </g>

      {/* Letter N */}
      <g>
        {/* N - left vertical */}
        <line
          x1="105"
          y1="65"
          x2="105"
          y2="135"
          stroke="white"
          strokeWidth="12"
          strokeLinecap="round"
        />
        {/* N - diagonal */}
        <line
          x1="105"
          y1="65"
          x2="140"
          y2="135"
          stroke="white"
          strokeWidth="12"
          strokeLinecap="round"
        />
        {/* N - right vertical */}
        <line
          x1="140"
          y1="65"
          x2="140"
          y2="135"
          stroke="white"
          strokeWidth="12"
          strokeLinecap="round"
        />
      </g>

      {/* Decorative corner accents */}
      <circle cx="35" cy="35" r="3" fill="white" opacity="0.4" />
      <circle cx="165" cy="35" r="3" fill="white" opacity="0.4" />
      <circle cx="35" cy="165" r="3" fill="white" opacity="0.4" />
      <circle cx="165" cy="165" r="3" fill="white" opacity="0.4" />
    </svg>
  );
};

export default Logo;
