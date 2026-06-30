import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { Link } from 'react-router-dom';
import { useState } from 'react';
import { useRegister } from '../../hooks/useAuth';

const schema = z.object({
    name: z.string().min(1, 'Ime je obavezno'),
    surname: z.string().min(1, 'Prezime je obavezno'),
    email: z.string().min(1, 'Email je obavezan').email('Unesite validan email'),
    password: z.string().min(8, 'Lozinka mora imati najmanje 8 karaktera'),
    confirmPassword: z.string().min(1, 'Potvrdite lozinku'),
    address: z.string().min(1, 'Adresa je obavezna'),
    phone: z.string().optional(),
    hasChildren: z.boolean(),
    hasPets: z.boolean(),
    livingSpace: z.string().optional(),
    role: z.enum(['User', 'Volunteer'])
}).refine(data => data.password === data.confirmPassword, {
    message: 'Lozinke se ne poklapaju',
    path: ['confirmPassword']
});

type RegisterForm = z.infer<typeof schema>;

// Helper komponenta za polje — da ne dupliramo kod
function Field({
    label,
    error,
    children
}: {
    label: string;
    error?: string;
    children: React.ReactNode;
}) {
    return (
        <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">
                {label}
            </label>
            {children}
            {error && <p className="text-xs text-red-500 mt-1">{error}</p>}
        </div>
    );
}

const inputClass = (hasError: boolean) =>
    `w-full border rounded-lg px-3 py-2 text-sm outline-none transition-colors
     focus:border-orange-400 focus:ring-2 focus:ring-orange-100
     ${hasError ? 'border-red-400' : 'border-gray-300'}`;

export function RegisterPage() {
    const { mutate: register, isPending, error } = useRegister();
    const [selectedRole, setSelectedRole] = useState<'User' | 'Volunteer'>('User');

    const {
        register: field,
        handleSubmit,
        setValue,
        formState: { errors }
    } = useForm<RegisterForm>({
        resolver: zodResolver(schema),
        defaultValues: {
            role: 'User',
            hasChildren: false,
            hasPets: false
        }
    });

    const onSubmit = (data: RegisterForm) => register(data);

    const handleRoleChange = (role: 'User' | 'Volunteer') => {
        setSelectedRole(role);
        setValue('role', role);
    };

    return (
        <div className="min-h-screen bg-gray-50 flex items-center justify-center px-4 py-10">
            <div className="w-full max-w-lg">

                {/* Logo */}
                <div className="text-center mb-8">
                    <span className="text-4xl">🐾</span>
                    <h1 className="text-2xl font-bold text-gray-900 mt-2">
                        Kreiraj nalog
                    </h1>
                    <p className="text-gray-500 mt-1">
                        Pridruži se i pomozi životinjama
                    </p>
                </div>

                <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8">
                    <form onSubmit={handleSubmit(onSubmit)} noValidate>

                        {/* Izbor uloge */}
                        <div className="mb-6">
                            <p className="text-sm font-medium text-gray-700 mb-2">
                                Tip naloga
                            </p>
                            <div className="grid grid-cols-2 gap-3">
                                {/* User dugme */}
                                <button
                                    type="button"
                                    onClick={() => handleRoleChange('User')}
                                    className={`p-3 rounded-lg border-2 text-sm font-medium
                                        transition-colors text-left
                                        ${selectedRole === 'User'
                                            ? 'border-orange-500 bg-orange-50 text-orange-700'
                                            : 'border-gray-200 text-gray-600 hover:border-gray-300'}`}
                                >
                                    <div className="text-lg mb-1">👤</div>
                                    <div>Udomitelj</div>
                                    <div className="text-xs font-normal text-gray-400 mt-0.5">
                                        Tražim ljubimca
                                    </div>
                                </button>

                                {/* Volunteer dugme */}
                                <button
                                    type="button"
                                    onClick={() => handleRoleChange('Volunteer')}
                                    className={`p-3 rounded-lg border-2 text-sm font-medium
                                        transition-colors text-left
                                        ${selectedRole === 'Volunteer'
                                            ? 'border-orange-500 bg-orange-50 text-orange-700'
                                            : 'border-gray-200 text-gray-600 hover:border-gray-300'}`}
                                >
                                    <div className="text-lg mb-1">🤝</div>
                                    <div>Volonter</div>
                                    <div className="text-xs font-normal text-gray-400 mt-0.5">
                                        Pomažem azilu
                                    </div>
                                </button>
                            </div>
                            {/* Skriveni input za react-hook-form */}
                            <input type="hidden" {...field('role')} />
                        </div>

                        {/* Ime i prezime u redu */}
                        <div className="grid grid-cols-2 gap-4 mb-4">
                            <Field label="Ime" error={errors.name?.message}>
                                <input
                                    placeholder="Ana"
                                    className={inputClass(!!errors.name)}
                                    {...field('name')}
                                />
                            </Field>
                            <Field label="Prezime" error={errors.surname?.message}>
                                <input
                                    placeholder="Anić"
                                    className={inputClass(!!errors.surname)}
                                    {...field('surname')}
                                />
                            </Field>
                        </div>

                        {/* Email */}
                        <div className="mb-4">
                            <Field label="Email" error={errors.email?.message}>
                                <input
                                    type="email"
                                    placeholder="tvoj@email.com"
                                    className={inputClass(!!errors.email)}
                                    {...field('email')}
                                />
                            </Field>
                        </div>

                        {/* Lozinke */}
                        <div className="grid grid-cols-2 gap-4 mb-4">
                            <Field label="Lozinka" error={errors.password?.message}>
                                <input
                                    type="password"
                                    placeholder="••••••••"
                                    className={inputClass(!!errors.password)}
                                    {...field('password')}
                                />
                            </Field>
                            <Field label="Potvrda lozinke" error={errors.confirmPassword?.message}>
                                <input
                                    type="password"
                                    placeholder="••••••••"
                                    className={inputClass(!!errors.confirmPassword)}
                                    {...field('confirmPassword')}
                                />
                            </Field>
                        </div>

                        {/* Adresa */}
                        <div className="mb-4">
                            <Field label="Adresa" error={errors.address?.message}>
                                <input
                                    placeholder="Ulica i broj, Grad"
                                    className={inputClass(!!errors.address)}
                                    {...field('address')}
                                />
                            </Field>
                        </div>

                        {/* Telefon */}
                        <div className="mb-4">
                            <Field label="Telefon (opciono)" error={errors.phone?.message}>
                                <input
                                    placeholder="06X XXX XXXX"
                                    className={inputClass(!!errors.phone)}
                                    {...field('phone')}
                                />
                            </Field>
                        </div>

                        {/* Checkbox polja */}
                        <div className="mb-6">
                            <p className="text-sm font-medium text-gray-700 mb-2">
                                O tebi
                            </p>
                            <div className="space-y-2">
                                <label className="flex items-center gap-2 cursor-pointer">
                                    <input
                                        type="checkbox"
                                        className="w-4 h-4 accent-orange-500"
                                        {...field('hasChildren')}
                                    />
                                    <span className="text-sm text-gray-600">
                                        Imam decu
                                    </span>
                                </label>
                                <label className="flex items-center gap-2 cursor-pointer">
                                    <input
                                        type="checkbox"
                                        className="w-4 h-4 accent-orange-500"
                                        {...field('hasPets')}
                                    />
                                    <span className="text-sm text-gray-600">
                                        Imam druge ljubimce
                                    </span>
                                </label>
                            </div>
                        </div>

                        {/* Greška sa servera */}
                        {error && (
                            <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg">
                                <p className="text-sm text-red-600">
                                    {(error as any)?.response?.data?.message
                                        ?? 'Greška pri registraciji. Pokušaj ponovo.'}
                                </p>
                            </div>
                        )}

                        {/* Submit */}
                        <button
                            type="submit"
                            disabled={isPending}
                            className="w-full bg-orange-500 hover:bg-orange-600 text-white
                                font-medium py-2.5 rounded-lg transition-colors
                                disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                            {isPending ? 'Kreiranje naloga...' : 'Kreiraj nalog'}
                        </button>
                    </form>

                    <p className="text-sm text-gray-500 text-center mt-6">
                        Već imaš nalog?{' '}
                        <Link
                            to="/login"
                            className="text-orange-500 hover:text-orange-600 font-medium"
                        >
                            Prijavi se
                        </Link>
                    </p>
                </div>
            </div>
        </div>
    );
}